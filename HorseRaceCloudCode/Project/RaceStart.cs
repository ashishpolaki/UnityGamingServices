using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HorseRaceCloudCode
{
    public class RaceStart
    {
        private readonly IGameApiClient gameApiClient;
        private readonly IPushClient pushClient;
        private readonly ILogger<RaceStart> _logger;

        public RaceStart(IGameApiClient _gameApiClient, IPushClient _pushClient, ILogger<RaceStart> logger)
        {
            this.gameApiClient = _gameApiClient;
            this.pushClient = _pushClient;
            this._logger = logger;
        }

        [CloudCodeFunction("StartRace")]
        public async Task<bool> StartRace(IExecutionContext context, string lobbyData)
        {
            List<RaceLobbyParticipant>? lobbyPlayers = JsonConvert.DeserializeObject<List<RaceLobbyParticipant>>(lobbyData);

            if(lobbyPlayers != null)
            {
                for (int i = 0; i < lobbyPlayers.Count; i++)
                {
                    await pushClient.SendPlayerMessageAsync(context, $"{lobbyPlayers[i].HorseNumber}", "RaceStart", lobbyPlayers[i].PlayerID);
                }

                //Set Race Lobby players in Cloud
                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                   context.PlayerId, new SetItemBody("RaceLobby", JsonConvert.SerializeObject(lobbyPlayers)));
            }
            return false;
        }

        [CloudCodeFunction("RaceResults")]
        public async Task SendRaceResultToPlayers(IExecutionContext context, int winnerHorseNumber)
        {
            List<RaceLobbyParticipant> raceLobbyData = await Utils.GetCustomDataWithKey<List<RaceLobbyParticipant>>(context, gameApiClient, context.PlayerId, "RaceLobby");

            for (int i = 0; i < raceLobbyData.Count; i++)
            {
                //Send Message to the players for Race Results
                await pushClient.SendPlayerMessageAsync(context, $"{winnerHorseNumber}", "RaceResult", raceLobbyData[i].PlayerID);
            }

            //Clear Lobby Data and Current Race Checkins
            await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                new SetItemBatchBody(new List<SetItemBody>()
                   {
                           new ("RaceLobby", ""),
                           new ("RaceCheckIn", "")
                   }));
        }
    }
}
