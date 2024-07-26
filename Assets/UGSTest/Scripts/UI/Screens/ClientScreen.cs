using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UGS.CloudSave;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private Button joinRaceBtn;
        [SerializeField] private Button backButton;
        [SerializeField] private Button raceStatusButton;
        [SerializeField] private TextMeshProUGUI raceStatusText;
        [SerializeField] private TextMeshProUGUI messageText;
        #endregion

        private bool isInRace = false;
        private bool isRaceResult = false;

        #region Unity Methods
        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(() => VenueCheckIn());
            joinRaceBtn.onClick.AddListener(() => EnterRace());
            backButton.onClick.AddListener(() => OnScreenBack());
            raceStatusButton.onClick.AddListener(() => OnRaceStatusHandle());
            GameManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            GameManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
            PlayerRaceStatus();
        }
        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
            joinRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
            raceStatusButton.onClick.RemoveAllListeners();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
                GameManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
            }
        }
        #endregion

        public override void OnScreenBack()
        {
            //If no tab is opened and the back button is pressed, then close screen
            if (CurrentOpenTab == ScreenTabType.None)
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
                Close();
                return;
            }
            base.OnScreenBack();

            if (CurrentOpenTab == ScreenTabType.None)
            {
                PlayerRaceStatus();
            }
        }

        private async void PlayerRaceStatus()
        {
            ResetData();
            await GameManager.Instance.FetchCurrentLocation();

            //Get host ID from the currentRaceCheckins location
            string hostID = await GameManager.Instance.GetHostID();
            if (string.IsNullOrEmpty(hostID))
            {
                return;
            }

            isRaceResult = await VerifyRaceResults();
            if (!isRaceResult)
            {
                isInRace = await VerifyRaceLobby();
            }
        }

        private void ResetData()
        {
            //Set Default Values
            isInRace = false;
            isRaceResult = false;
            raceStatusButton.gameObject.SetActive(false);
            joinRaceBtn.interactable = true;
        }

        private async Task<bool> VerifyRaceLobby()
        {
            //Check if the race lobby data exists. 
            string raceLobbyData = await GameManager.Instance.TryGetRaceLobby(GameManager.Instance.GameData.HostID);
            bool isRaceLobbyExists = !string.IsNullOrEmpty(raceLobbyData) && !string.IsNullOrWhiteSpace(raceLobbyData);
            bool isInRace = false;

            if (isRaceLobbyExists)
            {
                List<UGS.CloudSave.RaceLobbyParticipant> raceLobbyParticipants = JsonConvert.DeserializeObject<List<UGS.CloudSave.RaceLobbyParticipant>>(raceLobbyData);
                foreach (var lobbyParticipant in raceLobbyParticipants)
                {
                    if (lobbyParticipant.PlayerID == GameManager.Instance.GameData.PlayerID)
                    {
                        GameManager.Instance.GameData.HorseNumber = lobbyParticipant.HorseNumber;
                        isInRace = true;
                        break;
                    }
                }
            }

            //Check if the player is in race lobby. 
            if (isInRace)
            {
                raceStatusText.text = "Continue to Race";
                raceStatusButton.gameObject.SetActive(true);
                joinRaceBtn.interactable = false;
                return true;
            }
            return false;
        }

        private async Task<bool> VerifyRaceResults()
        {
            string playerRaceResultData = await GameManager.Instance.TryGetPlayerRaceResult();
            bool canSeeRaceResults = false;
            if (!string.IsNullOrEmpty(playerRaceResultData) && !string.IsNullOrWhiteSpace(playerRaceResultData))
            {
                UGS.CloudSave.PlayerRaceResult playerRaceResult = JsonConvert.DeserializeObject<UGS.CloudSave.PlayerRaceResult>(playerRaceResultData);
                if (playerRaceResult.PlayerID == GameManager.Instance.GameData.PlayerID)
                {
                    GameManager.Instance.GameData.RaceResult = playerRaceResult;
                    canSeeRaceResults = true;
                }
            }
            if (canSeeRaceResults)
            {
                raceStatusText.text = "See Race Results";
                raceStatusButton.gameObject.SetActive(true);
                return true;
            }
            return false;
        }

        private void OnRaceStart(string message)
        {
            GameManager.Instance.GameData.HorseNumber = int.Parse(message);
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceInProgress);
        }

        private void OnRaceResult(string _raceResult)
        {
            UGS.CloudSave.PlayerRaceResult raceResult = JsonConvert.DeserializeObject<UGS.CloudSave.PlayerRaceResult>(_raceResult);
            GameManager.Instance.GameData.RaceResult = raceResult;

            CloseAllTabs();
            OpenTab(ScreenTabType.RaceResults);
        }

        private void OnRaceStatusHandle()
        {
            if (isRaceResult)
            {
                OpenTab(ScreenTabType.RaceResults);
            }
            else if (isInRace)
            {
                OpenTab(ScreenTabType.RaceInProgress);
            }
        }

        private async void EnterRace()
        {
            await GameManager.Instance.FetchCurrentLocation();

            string dateTime = string.Empty;
            messageText.text = string.Empty;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
            }

            //Get host ID from the currentRaceCheckins location
            string hostID = await GameManager.Instance.GetHostID();
            if (string.IsNullOrEmpty(hostID) || string.IsNullOrWhiteSpace(hostID))
            {
                messageText.text = "No venue found at this location";
                return;
            }
            if (hostID == GameManager.Instance.GameData.PlayerID)
            {
                messageText.text = "Host can't join its own venue";
                return;
            }

            //Check if the race lobby data exists. 
            string raceLobbyData = await GameManager.Instance.TryGetRaceLobby(GameManager.Instance.GameData.HostID);
            bool isRaceLobbyExists = !string.IsNullOrEmpty(raceLobbyData) && !string.IsNullOrWhiteSpace(raceLobbyData);
            if (isRaceLobbyExists)
            {
                messageText.text = "Race is currently in progress. Please wait to join the next race.";
                return;
            }

            //Request Host to Enter the race
            RequestRaceJoinAsync(hostID, dateTime);
        }

        private async void VenueCheckIn()
        {
            await GameManager.Instance.FetchCurrentLocation();

            string dateTime = string.Empty;
            messageText.text = string.Empty;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
            }

            //Get host ID from the currentRaceCheckins location
            string hostID = await GameManager.Instance.GetHostID();
            if (string.IsNullOrEmpty(hostID))
            {
                messageText.text = "No venue found at this location";
                return;
            }
            if (hostID == GameManager.Instance.GameData.PlayerID)
            {
                messageText.text = "Host can't join its own venue";
                return;
            }

            Func<Task<string>> checkInResponse = () => GameManager.Instance.CloudCode.CheckIn(hostID, dateTime);
            string checkInMessage = await LoadingScreen.Instance.PerformAsyncWithLoading(checkInResponse);
            messageText.text = checkInMessage;
        }

        private async void RequestRaceJoinAsync(string hostID, string dateTime)
        {
            Func<Task<string>> raceJoinResponse = () => GameManager.Instance.CloudCode.RequestRaceJoin(hostID, dateTime);
            string raceJoin = await LoadingScreen.Instance.PerformAsyncWithLoading(raceJoinResponse);

            if (!string.IsNullOrEmpty(raceJoin))
            {
                UGS.CloudCode.JoinRaceResponse joinRaceResponse = JsonConvert.DeserializeObject<UGS.CloudCode.JoinRaceResponse>(raceJoin);
                DateTime raceTime = DateTime.Parse(joinRaceResponse.RaceTime);
                GameManager.Instance.GameData.RaceTime = raceTime;
                if (joinRaceResponse.CanWaitInLobby)
                {
                    // Show the lobby screen
                    OpenTab(ScreenTabType.Lobby);
                }
                else
                {
                    messageText.text = joinRaceResponse.Message;
                }
            }
        }
    }
}
