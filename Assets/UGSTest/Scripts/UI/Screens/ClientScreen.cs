using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private Button joinRaceBtn;
        [SerializeField] private Button backButton;
        [SerializeField] private Button continueRaceButton;
        [SerializeField] private TextMeshProUGUI messageText;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(() => CheckIn());
            joinRaceBtn.onClick.AddListener(() => JoinRace());
            backButton.onClick.AddListener(() => OnScreenBack());
            continueRaceButton.onClick.AddListener(() => ContinueRace());
            GameManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            GameManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
            PlayerRaceStatus();
        }
        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
            joinRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
            continueRaceButton.onClick.RemoveAllListeners();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
                GameManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
            }
        }
        private void Start()
        {
            GameManager.Instance.FetchCurrentLocation();
        }
        #endregion

        public override void OnScreenBack()
        {
            base.OnScreenBack();
            if (!CantGoBack)
            {
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
                Close();
            }
            if (!IsAnyTabOpened && CantGoBack)
            {
                PlayerRaceStatus();
            }
        }
        private async void PlayerRaceStatus()
        {
            //Get host ID from the currentRaceCheckins location
            string hostID = await GameManager.Instance.GetHostID();
            if (string.IsNullOrEmpty(hostID))
            {
                return;
            }

            //Check if the player is in race lobby. 
            string raceLobbyData = await GameManager.Instance.TryGetRaceLobbyData();
            bool isInRace = !string.IsNullOrEmpty(raceLobbyData) && !string.IsNullOrWhiteSpace(raceLobbyData);
            Debug.Log(raceLobbyData);
            if (isInRace)
            {
                UGS.CloudSave.RaceLobbyParticipant raceLobbyParticipant = JsonConvert.DeserializeObject<UGS.CloudSave.RaceLobbyParticipant>(raceLobbyData);
                GameManager.Instance.GameData.HorseNumber = raceLobbyParticipant.HorseNumber;
            }

            continueRaceButton.gameObject.SetActive(isInRace);
            joinRaceBtn.interactable = !isInRace;
        }
        private void OnRaceStart(string message)
        {
            GameManager.Instance.GameData.HorseNumber = int.Parse(message);
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceInProgress);
        }
        private void OnRaceResult(int horseNumber)
        {
            GameManager.Instance.GameData.WinnerHorseNumber = horseNumber;
            CloseAllTabs();
            OpenTab(ScreenTabType.RaceResults);
        }
        private void ContinueRace()
        {
            OpenTab(ScreenTabType.RaceInProgress);
        }
        private async void CheckIn()
        {
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

        private async void JoinRace()
        {
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
            await RequestRaceJoinAsync(hostID, dateTime);
        }


        private async Task RequestRaceJoinAsync(string hostID, string dateTime)
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
