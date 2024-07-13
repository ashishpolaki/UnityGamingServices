using Newtonsoft.Json;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;

namespace UGS
{
    public class CloudCode
    {
        public class RegisterHostItem
        {
            public string PlayerID { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public float Radius { get; set; }
        }
        public class RaceSchedule
        {
            public string ScheduleStart { get; set; }
            public string ScheduleEnd { get; set; }
            public string TimeGap { get; set; }
            public string PreRaceWaitTime { get; set; }
        }
        public class CheckInRequest
        {
            public string PlayerID { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        private HorseRaceCloudCodeBindings module;

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

        public async Task RegisterVenue(RegisterHostItem registerHostItem)
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
        public async void ScheduleRaceTime(string playerID, RaceSchedule raceSchedule)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(raceSchedule);
                await module.ScheduleRaceTimings(playerID, jsonData);
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
        }
        public async Task<string> CheckIn(CheckInRequest checkInRequest)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(checkInRequest);
                var result = await module.VenueCheckIn(jsonData);
                return result;
            }
            catch (CloudCodeException exception)
            {
                Debug.LogException(exception);
            }
            return null;
        }
    }

}
