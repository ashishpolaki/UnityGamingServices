using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                           new ("RaceSchedule",""),
                           new ("Players", "")
                   }));
            }
        }

        [CloudCodeFunction("RaceScheduleTimings")]
        public async Task ScheduleRaceTimings(IExecutionContext context, string playerID, string raceData)
        {
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       playerID, new SetItemBody("RaceSchedule", raceData));
        }

    }
}
