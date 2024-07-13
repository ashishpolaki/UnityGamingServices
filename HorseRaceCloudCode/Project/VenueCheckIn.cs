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
    public class VenueCheckIn
    {
        private readonly IGameApiClient gameApiClient;
        private readonly IPushClient pushClient;
        private readonly ILogger<VenueCheckIn> _logger;

        public VenueCheckIn(IGameApiClient _gameApiClient, IPushClient _pushClient, ILogger<VenueCheckIn> logger)
        {
            this.gameApiClient = _gameApiClient;
            this.pushClient = _pushClient;
            this._logger = logger;
        }

        [CloudCodeFunction("CheckedInVenue")]
        public async Task<string> CheckInVenue(IExecutionContext context, string checkInData)
        {
            var getResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId,
               "HostVenue");

            CheckInResponse? checkInResponse = JsonConvert.DeserializeObject<CheckInResponse>(checkInData);

            // Check if there is any data in HostVenue and checkInData is not null
            if (getResponse.Data.Results.Count > 0 && checkInResponse != null)
            {
                foreach (var item in getResponse.Data.Results)
                {
                    try
                    {
                        string? jsonString = item.Value.ToString();
                        if (jsonString != null)
                        {
                            RegisterHostItem? GetRegisterHostItem = JsonConvert.DeserializeObject<RegisterHostItem>(jsonString);
                            if (GetRegisterHostItem != null)
                            {
                                float distance = DistanceCalculator.CalculateHaversineDistance(GetRegisterHostItem.Latitude, GetRegisterHostItem.Longitude, checkInResponse.Latitude, checkInResponse.Longitude);
                                if (distance <= GetRegisterHostItem.Radius)
                                {
                                    await AddPlayerIntoHost(context, GetRegisterHostItem.PlayerID, checkInResponse.PlayerID);
                                    string message = await RequestCheckIn(context, GetRegisterHostItem.PlayerID, checkInResponse.PlayerID);
                                    return message;
                                }
                            }
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        _logger.LogError($"Error deserializing RegisterResponse: {ex.Message}");
                    }
                }
            }
            // he is not in venue location return
            return "Not in Venue Location";
        }


        #region Add Players to Host
        private async Task AddPlayerIntoHost(IExecutionContext context, string hostID, string playerID)
        {
            //Get all the players in the host 
            List<string>? players = await GetPlayersListFromHost(context, hostID);

            //If playerid does not exist in the list, then add it
            if (!players.Contains(playerID))
            {
                //Add the new player to the list
                players?.Add(playerID);

                //Update the list in the cloud
                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                 hostID, new SetItemBody("Players", JsonConvert.SerializeObject(players)));
            }

        }
        private async Task<List<string>> GetPlayersListFromHost(IExecutionContext context, string hostID)
        {
            List<string>? players = new List<string>();
            var getResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, hostID, new List<string> { "Players" });
            if (getResponse.Data.Results.Count > 0)
            {
                string? jsonString = getResponse.Data.Results[0].Value.ToString();
                if (jsonString != null)
                {
                    players = JsonConvert.DeserializeObject<List<string>>(jsonString);
                    if (players == null)
                    {
                        players = new List<string>();
                    }
                }
            }
            return players;
        }
        #endregion

        #region RequestCheckIn
        private async Task<string> RequestCheckIn(IExecutionContext context, string hostID, string playerId)
        {
            DateTime currentDateTime = DateTime.Now;
            bool IsCurrentDateExist = false;
            string key = $"{hostID}{currentDateTime.Year.ToString("YYYY")}{currentDateTime.Month.ToString("MM")}";
            string message = "Successfully Checked In";

            //Get the player checkin records from the cloud
            List<CheckInAttendance>? checkInRecords = await GetCheckInAttendancesListFromCloud(context, playerId, key);

            //If the list is greater than 0, then check if the current date is already in the list.
            if (checkInRecords.Count > 0)
            {
                foreach (var currentCheckInItem in checkInRecords)
                {
                    //If the current date matches the date in the list, then update the checkin record.
                    if (currentCheckInItem.Date == currentDateTime.Date.ToString("MM-dd"))
                    {
                        message = UpdateExistingDateCheckin(currentCheckInItem, currentDateTime);
                        IsCurrentDateExist = true;
                        break;
                    }
                }
            }

            //If the current date is not in the list, then create a new checkin record
            if (!IsCurrentDateExist || checkInRecords.Count == 0)
            {
                checkInRecords.Add(CreateNewCheckInAttendance(currentDateTime));
            }

            await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId,
                                 playerId, new SetItemBody(key, JsonConvert.SerializeObject(checkInRecords)));
            return message;
        }

        private string UpdateExistingDateCheckin(CheckInAttendance currentCheckInItem, DateTime currentDateTime)
        {
            //Parse the last checkin time.
            DateTime lastCheckInDateTime = DateTime.ParseExact(currentCheckInItem.LastCheckInTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);
            int currentInterval = (currentDateTime.Hour * 60 + currentDateTime.Minute) / 15;
            int lastCheckInInterval = (lastCheckInDateTime.Hour * 60 + lastCheckInDateTime.Minute) / 15;

            //If the current interval is not the same as the last checkin interval
            if (currentInterval != lastCheckInInterval)
            {
                currentCheckInItem.Count++;
                currentCheckInItem.LastCheckInTime = $"{currentDateTime.Hour.ToString("D2")}:{currentDateTime.Minute.ToString("D2")}";
                return "Successfully Checked In";
            }
            else
            {
                // Tell the player when they can check in next.
                DateTime nextCheckInTime = currentDateTime.AddMinutes(15 - (currentDateTime.Minute % 15)).AddSeconds(-currentDateTime.Second);
                TimeSpan timeUntilNextCheckIn = nextCheckInTime - currentDateTime;
                int totalSeconds = (int)timeUntilNextCheckIn.TotalSeconds;
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                return $"Next check-in is after {minutes} minutes and {seconds} seconds.";
            }
        }

        private CheckInAttendance CreateNewCheckInAttendance(DateTime currentDateTime)
        {
            CheckInAttendance checkInAttendance = new CheckInAttendance();
            checkInAttendance.LastCheckInTime = $"{currentDateTime.Hour.ToString("D2")}:{currentDateTime.Minute.ToString("D2")}";
            checkInAttendance.Count = 1;
            checkInAttendance.Date = currentDateTime.Date.ToString("MM-dd");
            return checkInAttendance;
        }

        private async Task<List<CheckInAttendance>> GetCheckInAttendancesListFromCloud(IExecutionContext context, string playerId, string key)
        {
            List<CheckInAttendance>? checkInRecords = new List<CheckInAttendance>();
            var response = await gameApiClient.CloudSaveData.GetProtectedItemsAsync(context, context.ServiceToken, context.ProjectId, playerId, new List<string> { key });
            if (response != null && response.Data.Results.Count > 0)
            {
                //Check the first item in the list and get the value
                string? jsonString = response.Data.Results[0].Value.ToString();
                if (jsonString != null)
                {
                    //Get the list of checkinrecord class. If it is null, then create a new list
                    checkInRecords = JsonConvert.DeserializeObject<List<CheckInAttendance>>(jsonString);
                    if (checkInRecords == null)
                    {
                        checkInRecords = new List<CheckInAttendance>();
                    }
                }
            }
            return checkInRecords;
        }
        #endregion

    }
}
