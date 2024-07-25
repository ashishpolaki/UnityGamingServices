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
        private readonly ILogger<VenueCheckIn> _logger;

        public VenueCheckIn(IGameApiClient _gameApiClient,  ILogger<VenueCheckIn> logger)
        {
            this.gameApiClient = _gameApiClient;
            this._logger = logger;
        }

        [CloudCodeFunction("CheckedInVenue")]
        public async Task<string> CheckInVenue(IExecutionContext context,string hostId,string dateTimeString)
        {
            DateTime dateTime = Utils.ParseDateTime(dateTimeString);

            //If the playerId and hostID are not empty, then proceed with the checkin process
            if (!string.IsNullOrEmpty(context.PlayerId) && !string.IsNullOrEmpty(hostId))
            {
                //The Player is in host location, request checkin 
                string checkInMessage = await RequestCheckIn(context, hostId, dateTime);
                return checkInMessage;
            }
            return "Not in Venue Location";
        }

        #region Request CheckIn
        private async Task<string> RequestCheckIn(IExecutionContext context, string hostID, DateTime currentDateTime)
        {
            bool IsCurrentDateExist = false;
            string key = $"{hostID}{currentDateTime.Year.ToString("D4")}{currentDateTime.Month.ToString("D2")}";
            string message = "Successfully Checked In";

            //Get the player checkin records from the cloud
            List<PlayerCheckIn>? playerCheckInsList = await Utils.GetProtectedDataWithKey<List<PlayerCheckIn>>(context, gameApiClient, context.PlayerId, key);

            //If the list is greater than 0, then check if the current date is already in the list.
            if (playerCheckInsList.Count > 0)
            {
                foreach (var currentCheckInItem in playerCheckInsList)
                {
                    //If the current date matches the date in the list, then update the checkin record.
                    if (currentCheckInItem.Date == currentDateTime.Date.ToString("dd"))
                    {
                        message = UpdateExistingDateCheckin(currentCheckInItem, currentDateTime);
                        IsCurrentDateExist = true;
                        break;
                    }
                }
            }

            //If the current date is not in the list, then add the player's first checkin.
            if (!IsCurrentDateExist || playerCheckInsList.Count == 0)
            {
                playerCheckInsList.Add(PlayerFirstCheckIn(currentDateTime));
            }

            await gameApiClient.CloudSaveData.SetProtectedItemAsync(context, context.ServiceToken, context.ProjectId,
                                 context.PlayerId, new SetItemBody(key, JsonConvert.SerializeObject(playerCheckInsList)));
            return message;
        }

        private string UpdateExistingDateCheckin(PlayerCheckIn currentCheckInItem, DateTime currentDateTime)
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

        /// <summary>
        /// First checkin of the player in the venue location.
        /// </summary>
        /// <param name="currentDateTime"></param>
        /// <returns></returns>
        private PlayerCheckIn PlayerFirstCheckIn(DateTime currentDateTime)
        {
            PlayerCheckIn checkInAttendance = new PlayerCheckIn();
            checkInAttendance.LastCheckInTime = $"{currentDateTime.Hour.ToString("D2")}:{currentDateTime.Minute.ToString("D2")}";
            checkInAttendance.Count = 1;
            checkInAttendance.Date = currentDateTime.Date.ToString("dd");
            return checkInAttendance;
        }
        #endregion
    }
}
