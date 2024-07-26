using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode.Subscriptions;
using UnityEngine;

namespace UGS
{
    public class CloudCode
    {
        #region Classes
        public class HostVenueData
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public float Radius { get; set; }
        }
        public class HostScheduleRace
        {
            public string ScheduleStart { get; set; }
            public string ScheduleEnd { get; set; }
            public int TimeGap { get; set; }
            public int PreRaceWaitTime { get; set; }
        }
        public class JoinRaceResponse
        {
            public bool CanWaitInLobby { get; set; }
            public string RaceTime { get; set; }
            public string Message { get; set; }
        }
        #endregion

        private HorseRaceCloudCodeBindings module;

        public event Action<string> OnRaceStarted;
        public event Action<string> OnRaceResult;

        public CloudCode()
        {
        }
        public async void InitializeBindings()
        {
            await Task.Delay(1000);
            module = new HorseRaceCloudCodeBindings(CloudCodeService.Instance);
        }
        public Task SubscribeToPlayerMessages()
        {
            var callbacks = new SubscriptionEventCallbacks();
            callbacks.MessageReceived += @event =>
            {
                switch (@event.MessageType)
                {
                    case "RaceStart":
                        OnRaceStarted?.Invoke(@event.Message);
                        break;
                    case "RaceResult":
                        OnRaceResult?.Invoke(@event.Message);
                        break;
                    default:
                        Debug.Log($"Got unsupported player Message:");
                        break;
                }
            };
            return CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(callbacks);
        }

        //Methods
        public async Task RegisterVenue(HostVenueData registerHostItem)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(registerHostItem);
                await module.RegisterVenue(jsonData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        public async Task ScheduleRaceTime(HostScheduleRace raceSchedule)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(raceSchedule);
                await module.ScheduleRaceTimings(jsonData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        public async Task<string> CheckIn(string hostID, string dateTime)
        {
            try
            {
                var result = await module.VenueCheckIn(hostID, dateTime);
                return result;
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return string.Empty;
        }

        public async Task<int> RaceJoin(string hostID, string dateTime)
        {
            int result = 0;
            try
            {
                result = await module.JoinRace(hostID, dateTime);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }

        public async Task<string> RequestRaceJoin(string hostID, string dateTime)
        {
            string result = string.Empty;
            try
            {
                result = await module.RequestRaceJoin(hostID, dateTime);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }
        public async Task<bool> ConfirmRaceCheckIn(string hostID, string playerName)
        {
            try
            {
                return await module.ConfirmRaceCheckIn(hostID, playerName);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }
        public async Task<bool> StartRace(string lobbyData,List<string> notQualifiedPlayersList)
        {
            try
            {
                return await module.StartRace(lobbyData, notQualifiedPlayersList);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return false;
        }
        public async Task SendRaceResults(string raceResultsData)
        {
            try
            {
                await module.SendRaceResults(raceResultsData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
    }

}
