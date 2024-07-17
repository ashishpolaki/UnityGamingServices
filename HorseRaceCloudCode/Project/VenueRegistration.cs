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

        public VenueRegistration(IGameApiClient _gameApiClient, IPushClient _pushClient, ILogger<VenueRegistration> logger)
        {
            this.gameApiClient = _gameApiClient;
        }

        [CloudCodeFunction("RegisterVenue")]
        public async Task SetVenue(IExecutionContext context, string venueData)
        {
            HostVenueData? registerItem = JsonConvert.DeserializeObject<HostVenueData>(venueData);
            if (registerItem != null)
            {
                //Register host data in VenuesList
                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       "HostVenue", new SetItemBody(context.PlayerId, venueData));

                //Venue Players
                await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
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
        public async Task ScheduleRaceTimings(IExecutionContext context, string raceData)
        {
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       context.PlayerId, new SetItemBody("RaceSchedule", raceData));
        }

    }
}
