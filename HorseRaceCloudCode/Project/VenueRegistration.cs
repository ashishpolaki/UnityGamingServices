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

        public VenueRegistration(IGameApiClient _gameApiClient)
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
                                      StringUtils.HOSTVENUEKEY, new SetItemBody(context.PlayerId, venueData));

                //If not data is present, set default data.
                var response = await gameApiClient.CloudSaveData.GetCustomItemsAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId);
                if (response.Data.Results.Count <= 0)
                {
                    //Venue GameData
                    await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                    new SetItemBatchBody(new List<SetItemBody>()
                       {
                           new (StringUtils.RACELOBBYKEY, ""),
                           new (StringUtils.RACESCHEDULEKEY,""),
                           new (StringUtils.RACECHECKINKEY, ""),
                           new (StringUtils.RACERESULTSKEY,"")
                       }));
                }
            }
        }

        [CloudCodeFunction("RaceScheduleTimings")]
        public async Task ScheduleRaceTimings(IExecutionContext context, string raceData)
        {
            await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                       context.PlayerId, new SetItemBody(StringUtils.RACESCHEDULEKEY, raceData));
        }

    }
}
