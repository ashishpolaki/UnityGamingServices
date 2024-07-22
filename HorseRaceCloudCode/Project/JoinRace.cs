using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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

            if (string.IsNullOrEmpty(getRaceScheduleData.ScheduleStart))
            {
                joinRaceResponse.Message = "Player Not updated Race Schedule Time";
                return JsonConvert.SerializeObject(joinRaceResponse);
            }

            //Get Upcoming Race Data
            _logger.LogDebug($"Get Upcoming {currentDateTime}");
            DateTime getRaceTime = GetUpcomingRaceData(getRaceScheduleData, currentDateTime);
            TimeSpan timeUntilNextRace = getRaceTime - currentDateTime;

            //Set JoinRace Response
            joinRaceResponse.RaceTime = getRaceTime;
            joinRaceResponse.CanWaitInLobby = timeUntilNextRace.TotalSeconds <= (getRaceScheduleData.PreRaceWaitTime * 60);
            if (joinRaceResponse.CanWaitInLobby)
            {
                //player can wait in the lobby
            }
            else
            {
                //Show the time the player can join the lobby
                int totalSeconds = (int)timeUntilNextRace.TotalSeconds;
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                timeUntilNextRace = timeUntilNextRace + new TimeSpan(0, -getRaceScheduleData.PreRaceWaitTime, 0);

                joinRaceResponse.Message = $"Player can join the lobby after {timeUntilNextRace.Hours.ToString("D2")}:{timeUntilNextRace.Minutes.ToString("D2")}:{timeUntilNextRace.Seconds.ToString("D2")}";
            }
            return JsonConvert.SerializeObject(joinRaceResponse);
        }

        private DateTime GetUpcomingRaceData(RaceScheduleData raceScheduleData, DateTime currentDateTime)
        {
            if (raceScheduleData != null)
            {
                // Parse the race start and end times
                DateTime raceStartTime = DateTime.ParseExact(raceScheduleData.ScheduleStart, "HH:mm", CultureInfo.InvariantCulture);
                DateTime raceEndTime = DateTime.ParseExact(raceScheduleData.ScheduleEnd, "HH:mm", CultureInfo.InvariantCulture);

                // If the end time is earlier in the day than the start time, add a day to the end time
                if (raceEndTime < raceStartTime)
                {
                    raceEndTime = raceEndTime.AddDays(1);
                }
                return GetNextRaceStartTime(currentDateTime, raceStartTime, raceEndTime, raceScheduleData.TimeGap);
            }
            return currentDateTime;
        }

        private DateTime GetNextRaceStartTime(DateTime currentTime, DateTime raceStartTime, DateTime raceEndTime, int intervalMinutes)
        {
            // Check if the current time is after the race day ends
            if (currentTime > raceEndTime)
            {
                return raceStartTime.AddDays(1);
            }
            else
            {
                //  TimeSpan timeUntilNextRace;
                if (currentTime <= raceStartTime)
                {
                    // If the current time is before the race starts, calculate the time until the race starts
                    return raceStartTime;
                }
                else
                {
                    // Calculate the time until this next race starts, including seconds
                    TimeSpan timeSinceRaceStart = currentTime - raceStartTime;
                    int totalSecondsSinceRaceStart = (int)timeSinceRaceStart.TotalSeconds;
                    int totalSecondsUntilNextRace = intervalMinutes * 60 - (totalSecondsSinceRaceStart % (intervalMinutes * 60));
                    DateTime nextRaceTime = currentTime.AddSeconds(totalSecondsUntilNextRace);

                    return nextRaceTime;
                }
            }
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
                    if (currentCheckInItem.Date == currentDateTime.Date.ToString("MM-dd"))
                    {
                        currentDayCheckIns = currentCheckInItem.Count;
                        break;
                    }
                }
            }

            //Add the player to the list
            raceCheckInPlayers.Add(new CurrentRacePlayerCheckIn() { PlayerID = context.PlayerId, PlayerName = playerName ,CurrentDayCheckIns = currentDayCheckIns } );

            //Save the updated list
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                         hostID, new SetItemBody("RaceCheckIn", JsonConvert.SerializeObject(raceCheckInPlayers)));

            return true;
        }
    }
}
