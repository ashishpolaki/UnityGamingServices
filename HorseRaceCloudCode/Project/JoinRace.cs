using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class JoinRace
    {
        private readonly IGameApiClient gameApiClient;
        private readonly ILogger<JoinRace> _logger;

        public JoinRace(IGameApiClient _gameApiClient, ILogger<JoinRace> logger)
        {
            this.gameApiClient = _gameApiClient;
            this._logger = logger;
        }

        [CloudCodeFunction("RaceJoinRequest")]
        public async Task<string> RaceJoinRequest(IExecutionContext context, string hostID, string dateTimeString)
        {
            JoinRaceResponse? joinRaceResponse = new JoinRaceResponse();
            DateTime currentDateTime = Utils.ParseDateTime(dateTimeString);

            var getRaceScheduleData = await Utils.GetCustomDataWithKey<RaceScheduleData>(context, gameApiClient, hostID, "RaceSchedule");

            //Check if the player has updated the Race Schedule Time
            if (string.IsNullOrEmpty(getRaceScheduleData.ScheduleStart))
            {
                joinRaceResponse.Message = "Player Not updated Race Schedule Time";
                return JsonConvert.SerializeObject(joinRaceResponse);
            }

            //Get upcoming race time
            DateTime raceStartTime = DateTime.ParseExact(getRaceScheduleData.ScheduleStart, "HH:mm", CultureInfo.InvariantCulture);
            DateTime raceEndTime = DateTime.ParseExact(getRaceScheduleData.ScheduleEnd, "HH:mm", CultureInfo.InvariantCulture);
            // If the end time is earlier in the day than the start time, add a day to the end time
            if (raceEndTime < raceStartTime)
            {
                raceEndTime = raceEndTime.AddDays(1);
            }

            List<DateTime> raceTimings = GetRaceTimings(raceStartTime, raceEndTime, TimeSpan.FromMinutes(getRaceScheduleData.TimeGap));
            DateTime? getRaceDateTime = GetUpcomingRaceTime(raceTimings, currentDateTime);

            if (getRaceDateTime != null)
            {
                DateTime raceDateTime = getRaceDateTime.Value;
                TimeSpan timeUntilNextRace = raceDateTime - currentDateTime;
                joinRaceResponse.RaceTime = raceDateTime;
                joinRaceResponse.CanWaitInLobby = timeUntilNextRace.TotalSeconds <= (getRaceScheduleData.PreRaceWaitTime * 60);

                if (!joinRaceResponse.CanWaitInLobby)
                {
                    //Show the time the player can join the lobby
                    timeUntilNextRace = timeUntilNextRace + new TimeSpan(0, -getRaceScheduleData.PreRaceWaitTime, 0);
                    joinRaceResponse.Message = $"Player can join the lobby after {timeUntilNextRace.Hours.ToString("D2")}:{timeUntilNextRace.Minutes.ToString("D2")}:{timeUntilNextRace.Seconds.ToString("D2")}";
                }
            }
            return JsonConvert.SerializeObject(joinRaceResponse);
        }


        private DateTime? GetUpcomingRaceTime(List<DateTime> raceTimings, DateTime currentTime)
        {
            if (currentTime > raceTimings.Last())
            {
                DateTime nextDayFirstRace = raceTimings.First().AddDays(1);
                return nextDayFirstRace;
            }
            foreach (var raceTime in raceTimings)
            {
                if (raceTime > currentTime)
                {
                    return raceTime;
                }
            }
            return null; // No upcoming race found
        }

        private List<DateTime> GetRaceTimings(DateTime startTime, DateTime endTime, TimeSpan interval)
        {
            List<DateTime> timings = new List<DateTime>();
            DateTime currentTime = startTime;

            while (currentTime <= endTime)
            {
                timings.Add(currentTime);
                currentTime = currentTime.Add(interval);
            }
            return timings;
        }


        [CloudCodeFunction("ConfirmRaceCheckIn")]
        public async Task<bool> ConfirmRaceCheckIn(IExecutionContext context, string hostID, string playerName)
        {
            //Get the list of players who have checked in
            List<CurrentRacePlayerCheckIn>? raceCheckInPlayers = await Utils.GetCustomDataWithKey<List<CurrentRacePlayerCheckIn>>(context, gameApiClient, hostID, "RaceCheckIn");

            //Get current day player checkins for the player
            int currentDayCheckIns = 0;
            DateTime currentDateTime = DateTime.UtcNow;
            string key = $"{hostID}{currentDateTime.Year.ToString("YYYY")}{currentDateTime.Month.ToString("MM")}";

            List<PlayerCheckIn>? playerCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerCheckIn>>(context, gameApiClient, context.PlayerId, key);

            //If the list is greater than 0, then check if the current date is already in the list.
            if (playerCheckInsList.Count > 0)
            {
                foreach (var currentCheckInItem in playerCheckInsList)
                {
                    //If the current date matches the date in the list, then update the checkin record.
                    if (currentCheckInItem.Date == currentDateTime.Date.ToString("dd"))
                    {
                        currentDayCheckIns = currentCheckInItem.Count;
                        break;
                    }
                }
            }

            //Add the player to the list
            raceCheckInPlayers.Add(new CurrentRacePlayerCheckIn() { PlayerID = context.PlayerId, PlayerName = playerName, CurrentDayCheckIns = currentDayCheckIns });

            //Save the updated list
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                         hostID, new SetItemBody("RaceCheckIn", JsonConvert.SerializeObject(raceCheckInPlayers)));

            return true;
        }
    }
}
