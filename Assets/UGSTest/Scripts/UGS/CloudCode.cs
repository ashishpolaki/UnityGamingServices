using Newtonsoft.Json;
using System;
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
        public event Action<int> OnRaceResult;

        public CloudCode()
        {
            // create a new instance of the module
            InitializeBindings();
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
                        OnRaceResult?.Invoke(int.Parse(@event.Message));
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
        public async void ScheduleRaceTime(HostScheduleRace raceSchedule)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(raceSchedule);
                await module.ScheduleRaceTimings( jsonData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        public async Task<string> CheckIn(double latitude, double longitude,string dateTime)
        {
            try
            {
                var result = await module.VenueCheckIn(latitude, longitude,dateTime);
                return result;
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return string.Empty;
        }

        public async Task<int> RaceJoin(double latitude, double longitude, string dateTime)
        {
            int result = 0;
            try
            {
                result = await module.JoinRace(latitude, longitude, dateTime);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }

        public async Task<string> RequestRaceJoin(double latitude, double longitude, string dateTime)
        {
            string result = string.Empty;
            try
            {
                result = await module.RequestRaceJoin(latitude, longitude, dateTime);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }
        public async Task<string> ConfirmRaceCheckIn(double latitude, double longitude)
        {
            string result = string.Empty;
            try
            {
                result = await module.ConfirmRaceCheckIn(latitude,longitude);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }
        public async Task<string> GetLobbyPlayers()
        {
            string result = string.Empty;
            try
            {
                result = await module.GetLobbyPlayers();
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return result;
        }
        public async Task StartRace(string horsesInRaceOrder)
        {
            try
            {
                await module.StartRace(horsesInRaceOrder);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        public async Task SendRaceResults(int winnerHorseNumber)
        {
            try
            {
                await module.SendRaceResults(winnerHorseNumber);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
    }

}
