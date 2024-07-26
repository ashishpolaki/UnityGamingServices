using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostLobbyTab : BaseTab
    {
        [SerializeField] private Button startRace_btn;
        [SerializeField] private TextMeshProUGUI checkInPlayerNamesTxt;

        private int randomPlayerCount = 11;
        private List<int> horsesInRaceOrderList = new List<int>();
        private Dictionary<string, UGS.CloudSave.CurrentRacePlayerCheckIn> currentRaceCheckins = new Dictionary<string, UGS.CloudSave.CurrentRacePlayerCheckIn>();
        private List<UGS.CloudSave.RaceLobbyParticipant> lobbyPlayerList = new List<UGS.CloudSave.RaceLobbyParticipant>();

        #region Unity Methods
        private void OnEnable()
        {
            startRace_btn.onClick.AddListener(() => StartRace());
            RaceStatus();
        }
        private void OnDisable()
        {
            startRace_btn.onClick.RemoveAllListeners();
        }
        #endregion

        private async void RaceStatus()
        {
            if (GameManager.Instance.GameData.IsRaceStart)
            {
                return;
            }
            GameManager.Instance.GameData.IsRaceStart = true;

            string playersRaceCheckIn = await HasPlayersCheckedIn();
            if (string.IsNullOrEmpty(playersRaceCheckIn))
            {
                return;
            }

            //Get horses in race order list.
            horsesInRaceOrderList = GameManager.Instance.HorsesInRaceOrderList;
            List<UGS.CloudSave.CurrentRacePlayerCheckIn> checkins = JsonConvert.DeserializeObject<List<UGS.CloudSave.CurrentRacePlayerCheckIn>>(playersRaceCheckIn);
            currentRaceCheckins = checkins.ToDictionary(checkin => checkin.PlayerID, checkin => checkin);

            await SetRaceWinner();
            SetRemainingRacePlayers();
            ShuffleLobbyPlayersList();
            DisplayLobbyPlayers();

            //PlayerLobbyStatus button if more than 2 players are checked in.
            // startRace_btn.interactable = (currentRaceCheckins.Count > 0);
        }

        private async Task<string> HasPlayersCheckedIn()
        {
            //Check in players
            Func<Task<string>> response = () => GameManager.Instance.GetRaceCheckInParticipants();
            string data = await LoadingScreen.Instance.PerformAsyncWithLoading(response);

            //If no players are checked in, display message and return.
            if (string.IsNullOrEmpty(data))
            {
                checkInPlayerNamesTxt.text = "No Players Checked In";
                return string.Empty;
            }

            return data;
        }

        private void DisplayLobbyPlayers()
        {
            if (lobbyPlayerList.Count > 0)
            {
                checkInPlayerNamesTxt.text = string.Join("\n", lobbyPlayerList.Select(x => $"{x.PlayerName} - Horse #{x.HorseNumber}"));
            }
        }
        private void ShuffleLobbyPlayersList()
        {
            System.Random random = new System.Random();
            int n = lobbyPlayerList.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

                UGS.CloudSave.RaceLobbyParticipant temp = lobbyPlayerList[i];
                lobbyPlayerList[i] = lobbyPlayerList[j];
                lobbyPlayerList[j] = temp;
            }
        }
        private async Task SetRaceWinner()
        {
            string winnerKey = await GetWeightedRandomWinner();
            AddPlayerToLobby(winnerKey);
        }

        private void SetRemainingRacePlayers()
        {
            if (currentRaceCheckins.Count < randomPlayerCount)
            {
                randomPlayerCount = currentRaceCheckins.Count;
            }
            for (int i = 0; i < randomPlayerCount; i++)
            {
                string randomPlayerKey = GetRandomPlayer();
                AddPlayerToLobby(randomPlayerKey);
            }
        }
        private void AddPlayerToLobby(string playerKey)
        {
            lobbyPlayerList.Add(new UGS.CloudSave.RaceLobbyParticipant
            {
                PlayerID = playerKey,
                PlayerName = currentRaceCheckins[playerKey].PlayerName,
                HorseNumber = horsesInRaceOrderList[0]
            });

            horsesInRaceOrderList.RemoveAt(0);
            currentRaceCheckins.Remove(playerKey);
        }

        private async void StartRace()
        {
            string lobbyData = JsonConvert.SerializeObject(lobbyPlayerList);
            List<string> notQualifiedPlayersList = currentRaceCheckins.Keys.ToList();

            Func<Task<bool>> response = () => GameManager.Instance.CloudCode.StartRace(lobbyData, notQualifiedPlayersList);
            bool canStartRace = await LoadingScreen.Instance.PerformAsyncWithLoading(response);

            if (canStartRace)
            {
                UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Show, ScreenTabType.RaceInProgress);
                gameObject.SetActive(false);
            }
        }

        private async Task<string> GetWeightedRandomWinner()
        {
            System.Random random = new System.Random();
            double cumulative = 0.0;
            long totalCheckIns = await Task.Run(() => currentRaceCheckins.Sum(entry => (long)entry.Value.CurrentDayCheckIns));

            // Generate a random number between 0 and totalCheckIns
            double randomNumber = random.NextDouble() * totalCheckIns;
            // Select the winner based on the weighted random number
            foreach (var player in currentRaceCheckins)
            {
                cumulative += player.Value.CurrentDayCheckIns;
                if (randomNumber < cumulative)
                {
                    string winnerKey = player.Key;
                    return winnerKey;
                }
            }
            // In case something goes wrong, return a default value (shouldn't happen).
            return currentRaceCheckins.Keys.ElementAt(0);
        }

        private string GetRandomPlayer()
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(currentRaceCheckins.Count);
            string playerKey = currentRaceCheckins.Keys.ElementAt(randomNumber);
            return playerKey;
        }
    }
}
