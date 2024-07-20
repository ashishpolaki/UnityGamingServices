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
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private Button joinRaceBtn;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI messageText;

        [SerializeField] private double currentLocationLatitude;
        [SerializeField] private double currentLocationLongitude;

        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(() => CheckIn());
            joinRaceBtn.onClick.AddListener(() => JoinRace());
            backButton.onClick.AddListener(() => OnScreenBack());
            GameManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            GameManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
        }

        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
            joinRaceBtn.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
                GameManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
            }
        }
        private void Start()
        {
            FetchCurrentLocation();
        }

        public override void OnScreenBack()
        {
            base.OnScreenBack();
            if(!CantGoBack)
            {
                UIController.Instance.ScreenEvent(this.ScreenType,UIScreenEvent.Close);
                UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show);
            }
        }

        private void OnRaceStart(string message)
        {
            GameManager.Instance.GameData.SetHorseNumber(int.Parse(message));
            OpenTab(ScreenTabType.RaceInProgress);
        }
        private void OnRaceResult(int horseNumber)
        {
            GameManager.Instance.GameData.SetWinnerHorseNumber(horseNumber);
            OpenTab(ScreenTabType.RaceResults);
        }
        private async void FetchCurrentLocation()
        {
            bool result = await GameManager.Instance.GPS.TryGetLocationAsync();
            if (result)
            {
                currentLocationLatitude = GameManager.Instance.GPS.CurrentLocationLatitude;
                currentLocationLongitude = GameManager.Instance.GPS.CurrentLocationLongitude;
            }
        }
        private async void CheckIn()
        {
            string dateTime = string.Empty;
            messageText.text = string.Empty;

            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
                currentLocationLatitude = CheatCode.Instance.Latitude;
                currentLocationLongitude = CheatCode.Instance.Longitude;
            }
            var result = await GameManager.Instance.CloudCode.CheckIn(currentLocationLatitude, currentLocationLongitude, dateTime);
            messageText.text = result;
        }

        private async void JoinRace()
        {
            string dateTime = string.Empty;
            if (CheatCode.Instance.IsCheatEnabled)
            {
                dateTime = CheatCode.Instance.CheatDateTime;
                currentLocationLatitude = CheatCode.Instance.Latitude;
                currentLocationLongitude = CheatCode.Instance.Longitude;
            }
            bool isJoinedRace = await RaceJoinAsync(dateTime);
            if (isJoinedRace)
            {
                return;
            }
            await RequestRaceJoinAsync(dateTime);
        }
        private async Task<bool> RaceJoinAsync(string dateTime)
        {
            int responseData = await GameManager.Instance.CloudCode.RaceJoin(currentLocationLatitude, currentLocationLongitude, dateTime);
            if (responseData != 0)
            {
                OpenTab(ScreenTabType.RaceInProgress);
                GameManager.Instance.GameData.SetHorseNumber(responseData);
                return true;
            }
            return false;
        }

        private async Task RequestRaceJoinAsync(string dateTime)
        {
            string responseData = await GameManager.Instance.CloudCode.RequestRaceJoin(currentLocationLatitude, currentLocationLongitude, dateTime);
            if (!string.IsNullOrEmpty(responseData))
            {
                UGS.CloudCode.JoinRaceResponse joinRaceResponse = JsonConvert.DeserializeObject<UGS.CloudCode.JoinRaceResponse>(responseData);
                //DateTime raceTime = DateTime.ParseExact(joinRaceResponse.RaceTime, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                DateTime raceTime = DateTime.Parse(joinRaceResponse.RaceTime);

                GameManager.Instance.GameData.SetGameData(joinRaceResponse.CanWaitInLobby, raceTime);
                if (joinRaceResponse.CanWaitInLobby)
                {
                    // Show the lobby screen
                    OpenTab(ScreenTabType.LobbyWait);
                }
                else
                {
                    messageText.text = joinRaceResponse.Message;
                }
            }
            else
            {
                messageText.text = responseData;
            }
        }
    }
}
