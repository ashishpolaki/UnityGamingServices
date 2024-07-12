using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class VenueRegistration
    {
        private readonly IGameApiClient gameApiClient;
        private readonly IPushClient pushClient;
        private readonly ILogger<VenueRegistration> _logger;

        public VenueRegistration(IGameApiClient _gameApiClient, IPushClient _pushClient, ILogger<VenueRegistration> logger)
        {
            this.gameApiClient = _gameApiClient;
            this.pushClient = _pushClient;
            this._logger = logger;
        }

        [CloudCodeFunction("RegisterVenue")]
        public async Task SetVenue(IExecutionContext context, string venueData)
        {
            RegisterHostItem? registerItem = JsonConvert.DeserializeObject<RegisterHostItem>(venueData);
            if (registerItem != null)
            {
                //Register host data in VenuesList
                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       "HostVenue", new SetItemBody(registerItem.PlayerID, venueData));

                //Venue Players
                await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, registerItem.PlayerID,
                new SetItemBatchBody(new List<SetItemBody>()
                   {
                           new ("RaceJoinCode",""),
                           new ("RaceLobby", ""),
                           new ("RaceCheckIn", ""),
                           new ("Players", "")
                   }));
            }
        }

        [CloudCodeFunction("RaceScheduleTimings")]
        public async Task ScheduleRaceTimings(IExecutionContext context, string playerID, string raceData)
        {
        }

        [CloudCodeFunction("VenueCheckIn")]
        public async Task CheckInVenue(IExecutionContext context, string venueData)
        {
            var getResponse = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId,
               "HostVenue");

            if (getResponse.Data.Results.Count > 0)
            {
                foreach (var item in getResponse.Data.Results)
                {
                    try
                    {
                        string? jsonString = item.Value.ToString();
                        if (jsonString != null)
                        {
                            RegisterHostItem? registerHostItem = JsonConvert.DeserializeObject<RegisterHostItem>(jsonString);
                            if (registerHostItem != null)
                            {
                                _logger.LogDebug($"{registerHostItem.PlayerID} Latitude : {registerHostItem.Latitude} , Longitudde : {registerHostItem.Longitude} ");
                            }
                        }
                    }
                    catch (JsonReaderException ex)
                    {
                        _logger.LogError($"Error deserializing RegisterResponse: {ex.Message}");
                    }
                }
            }
        }
    }
    public class RegisterHostItem
    {
        public string PlayerID { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Radius { get; set; }
    }

    public class RaceData
    {
        public float raceStartTime;
        public float raceEndTime;
        public float raceInterval;
    }
}
