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

        [CloudCodeFunction("RaceJoin")]
        public async Task<int> RaceJoin(IExecutionContext context, double latitude, double longitude, string dateTimeString)
        {
            int horseNumber = 0;

            //Get host ID from player's current location
            string hostID = await Utils.GetHostID(context, gameApiClient, _logger, latitude, longitude);

            //If the playerId and hostID are not empty, then proceed with the checkin process
            if (!string.IsNullOrEmpty(context.PlayerId) && !string.IsNullOrEmpty(hostID))
            {

                List<RaceLobbyData> raceLobbyData = await Utils.GetCustomDataWithKey<List<RaceLobbyData>>(context, gameApiClient, hostID, "RaceLobby");

                if (raceLobbyData.Count > 0)
                {
                    foreach (var playerRaceLobbyData in raceLobbyData)
                    {
                        if (playerRaceLobbyData.PlayerID == context.PlayerId)
                        {
                            horseNumber = playerRaceLobbyData.HorseNumber;
                        }
                    }
                }
            }

            return horseNumber;
        }

        [CloudCodeFunction("RaceJoinRequest")]
        public async Task<string> RaceJoinRequest(IExecutionContext context, double latitude, double longitude, string dateTimeString)
        {
            JoinRaceResponse? joinRaceResponse = new JoinRaceResponse();
            DateTime currentDateTime = Utils.ParseDateTime(dateTimeString);

            //Get host ID from player's current location
            string hostID = await Utils.GetHostID(context, gameApiClient, _logger, latitude, longitude);


            //Return if the player is not in the host location
            if (string.IsNullOrEmpty(hostID))
            {
                joinRaceResponse.Message = "Player is not in Host Location";
                return JsonConvert.SerializeObject(joinRaceResponse);
            }

            //If the playerId and hostID are not empty, then proceed with the checkin process
            if (!string.IsNullOrEmpty(context.PlayerId) && !string.IsNullOrEmpty(hostID))
            {
                var getRaceScheduleData = await Utils.GetCustomDataWithKey<RaceScheduleData>(context, gameApiClient, hostID, "RaceSchedule");

                if (string.IsNullOrEmpty(getRaceScheduleData.ScheduleStart))
                {
                    joinRaceResponse.Message = "Player Not updated Race Schedule Time";
                    return JsonConvert.SerializeObject(joinRaceResponse);
                }

                //Get Upcoming Race Data
                DateTime getRaceTime = GetUpcomingRaceData(getRaceScheduleData, currentDateTime);
                TimeSpan timeUntilNextRace = getRaceTime - currentDateTime;

                //Set JoinRace Response
                joinRaceResponse.RaceTime = getRaceTime;
                joinRaceResponse.CanWaitInLobby = timeUntilNextRace.TotalSeconds <= (getRaceScheduleData.PreRaceWaitTime * 60);
                if (joinRaceResponse.CanWaitInLobby)
                {
                    joinRaceResponse.Message = "Player can wait in Lobby";
                }
                else
                {
                    int totalSeconds = (int)timeUntilNextRace.TotalSeconds;
                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;

                    timeUntilNextRace = timeUntilNextRace + new TimeSpan(0, -getRaceScheduleData.PreRaceWaitTime, 0);

                    joinRaceResponse.Message = $"Player can join the lobby after {timeUntilNextRace.Hours.ToString("D2")}:{timeUntilNextRace.Minutes.ToString("D2")}:{timeUntilNextRace.Seconds.ToString("D2")}";
                }
                return JsonConvert.SerializeObject(joinRaceResponse);
            }
            else
            {
                joinRaceResponse.Message = "Error in Player Data";
                return JsonConvert.SerializeObject(joinRaceResponse);
            }
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
                //  return "The race will start tomorrow.";
            }
            else
            {
                //  TimeSpan timeUntilNextRace;
                if (currentTime <= raceStartTime)
                {
                    // If the current time is before the race starts, calculate the time until the race starts
                    //  timeUntilNextRace = raceStartTime - currentTime;
                    return raceStartTime;
                }
                else
                {
                    TimeSpan timeSinceRaceStart = currentTime - raceStartTime;
                    int totalSecondsSinceRaceStart = (int)timeSinceRaceStart.TotalSeconds;
                    int totalSecondsUntilNextRace = intervalMinutes * 60 - (totalSecondsSinceRaceStart % (intervalMinutes * 60));
                    DateTime nextRaceTime = currentTime.AddSeconds(totalSecondsUntilNextRace);

                    // Calculate the time until this next race starts, including seconds
                    // timeUntilNextRace = nextRaceTime - currentTime;
                    return nextRaceTime;
                }
            }
        }

        [CloudCodeFunction("ConfirmRaceCheckIn")]
        public async Task<string> ConfirmRaceCheckIn(IExecutionContext context, double latitude, double longitude)
        {
            string hostID = await Utils.GetHostID(context, gameApiClient, _logger, latitude, longitude);

            //Return if the player is not in the host location
            if (string.IsNullOrEmpty(hostID))
            {
                return "Player is not in Host Location";
            }

            List<string>? raceCheckInPlayers = await Utils.GetCustomDataWithKey<List<string>>(context, gameApiClient, hostID, "RaceCheckIn");

            //Check if the player is already checked in
            if (raceCheckInPlayers.Contains(context.PlayerId))
            {
                return "Player Already Checked In";
            }

            //If player is not checked in, then add the player to the list
            if (!raceCheckInPlayers.Contains(context.PlayerId))
            {
                raceCheckInPlayers?.Add(context.PlayerId);

                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                             hostID, new SetItemBody("RaceCheckIn", JsonConvert.SerializeObject(raceCheckInPlayers)));
            }
            return "Player Checked In";
        }
    }
}
