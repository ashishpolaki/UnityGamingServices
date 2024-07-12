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
            public float Longitude { get; set; }
            public float Latitude { get; set; }
            public float Radius { get; set; }
        }
        public class RaceSchedule
        {
            public string ScheduleStart;
            public string ScheduleEnd;
            public string TimeGap;
            public string preRaceWaitTime;
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

        public async void RegisterVenue(RegisterHostItem registerHostItem)
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
        public async void ScheduleRaceTime(RaceSchedule raceSchedule)
        {
            //try
            //{
            //    string jsonData = JsonConvert.SerializeObject(raceSchedule);
            // //   await module.ScheduleRaceTimings( jsonData);
            //}
        }
        public async void CheckIn()
        {

        }
    }
    
}
