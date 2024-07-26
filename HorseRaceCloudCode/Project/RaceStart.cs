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
        public async Task<bool> StartRace(IExecutionContext context, string lobbyData, List<string> notQualifiedPlayersList)
        {
            List<RaceLobbyParticipant>? lobbyPlayers = JsonConvert.DeserializeObject<List<RaceLobbyParticipant>>(lobbyData);

            if (lobbyPlayers != null)
            {
                for (int i = 0; i < lobbyPlayers.Count; i++)
                {
                    await pushClient.SendPlayerMessageAsync(context, $"{lobbyPlayers[i].HorseNumber}", "RaceStart", lobbyPlayers[i].PlayerID);
                }

                //For unqualified players send zero as horse number
                foreach (var unQualifiedPlayer in notQualifiedPlayersList)
                {
                    await pushClient.SendPlayerMessageAsync(context, $"{0}", "RaceStart", unQualifiedPlayer);
                }

                //Set Race Lobby players in Cloud and reset RaceResults
                await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                   new SetItemBatchBody(new List<SetItemBody>()
                      {
                           new SetItemBody("RaceLobby", JsonConvert.SerializeObject(lobbyPlayers)),
                           new (StringUtils.RACERESULTSKEY,"")
                      }));
                return true;
            }
            return false;
        }

        [CloudCodeFunction("RaceResults")]
        public async Task SendRaceResultToPlayers(IExecutionContext context, string raceResultData)
        {
            RaceResult? raceResultsList = JsonConvert.DeserializeObject<RaceResult>(raceResultData);

            if (raceResultsList != null)
            {
                for (int i = 0; i < raceResultsList.playerRaceResults.Count; i++)
                {
                    //Send Message to the players for Race Results
                    await pushClient.SendPlayerMessageAsync(context, $"{raceResultsList.playerRaceResults[i]}", "RaceResult", raceResultsList.playerRaceResults[i].PlayerID);
                }
            }

            //Update RaceResults, Clear Lobby Data and Current Race Checkins
            await gameApiClient.CloudSaveData.SetCustomItemBatchAsync(context, context.ServiceToken, context.ProjectId, context.PlayerId,
                new SetItemBatchBody(new List<SetItemBody>()
                   {
                           new ("RaceLobby", ""),
                           new ("RaceCheckIn", ""),
                           new SetItemBody("RaceResults", JsonConvert.SerializeObject(raceResultsList))
                   }));
        }
    }
}
