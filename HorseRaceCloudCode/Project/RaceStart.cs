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

        [CloudCodeFunction("GetRaceCheckInPlayers")]
        public async Task<string> GetRaceCheckInPlayers(IExecutionContext context)
        {
            List<string>? players = await Utils.GetCustomDataWithKey<List<string>>(context, gameApiClient, context.PlayerId, "RaceCheckIn");

            if (players.Count > 0)
            {
                return JsonConvert.SerializeObject(players);
            }
            else
            {
                return "No player Joined the race";
            }
        }

        [CloudCodeFunction("StartRace")]
        public async Task StartRace(IExecutionContext context, string horsesInRaceOrder)
        {
            List<string>? players = await Utils.GetCustomDataWithKey<List<string>>(context, gameApiClient, context.PlayerId, "RaceCheckIn");
            List<int>? horseNumbersInRaceOrder = JsonConvert.DeserializeObject<List<int>>(horsesInRaceOrder);

            if (horseNumbersInRaceOrder != null)
            {
                //Adjust the horse numbers based on the number of players
                horseNumbersInRaceOrder = horseNumbersInRaceOrder.GetRange(0, players.Count);

                //Shuffle the horses 
                Random rng = new Random();
                int n = horseNumbersInRaceOrder.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    int value = horseNumbersInRaceOrder[k];
                    horseNumbersInRaceOrder[k] = horseNumbersInRaceOrder[n];
                    horseNumbersInRaceOrder[n] = value;
                }

                //Send messages to the selected players with their horse numbers to start the race.
                List<RaceLobbyData> raceLobbyData = new List<RaceLobbyData>();
                for (int i = 0; i < players.Count; i++)
                {
                    raceLobbyData.Add(new RaceLobbyData()
                    {
                        PlayerID = players[i],
                        HorseNumber = horseNumbersInRaceOrder[i]
                    });
                    await pushClient.SendPlayerMessageAsync(context, $"{horseNumbersInRaceOrder[i]}", "RaceStart", players[i]);
                }

                //Set Race Lobby players in Cloud
                await gameApiClient.CloudSaveData.SetCustomItemAsync(context, context.ServiceToken, context.ProjectId,
                                   context.PlayerId, new SetItemBody("RaceLobby", JsonConvert.SerializeObject(raceLobbyData)));
            }
        }

        [CloudCodeFunction("RaceResults")]
        public async Task SendRaceResultToPlayers(IExecutionContext context, int winnerHorseNumber)
        {
            List<RaceLobbyData> raceLobbyData = await Utils.GetCustomDataWithKey<List<RaceLobbyData>>(context, gameApiClient, context.PlayerId, "RaceLobby");

            for (int i = 0; i < raceLobbyData.Count; i++)
            {
                //Send Message to the players for Race Results
                await pushClient.SendPlayerMessageAsync(context, $"{winnerHorseNumber}", "RaceResult", raceLobbyData[i].PlayerID);
            }
        }
    }
}
