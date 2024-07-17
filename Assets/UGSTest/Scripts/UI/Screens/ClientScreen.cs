using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private Button joinRaceBtn;
        [SerializeField] private TextMeshProUGUI messageText;

        [SerializeField] private double currentLocationLatitude;
        [SerializeField] private double currentLocationLongitude;

        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(() => CheckIn());
            joinRaceBtn.onClick.AddListener(() => JoinRace());
            GameManager.Instance.CloudCode.OnRaceStarted += OnRaceStart;
            GameManager.Instance.CloudCode.OnRaceResult += OnRaceResult;
        }

        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
            joinRaceBtn.onClick.RemoveAllListeners();
            GameManager.Instance.CloudCode.OnRaceStarted -= OnRaceStart;
            GameManager.Instance.CloudCode.OnRaceResult -= OnRaceResult;
        }
        private void Start()
        {
            FetchCurrentLocation();
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
            currentLocationLatitude = 17.48477376610915;
            currentLocationLongitude = 78.41440387735862;
        }
        private async void CheckIn()
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                messageText.text = await CheatCode.Instance.CheckIn();
                return;
            }

            var result = await GameManager.Instance.CloudCode.CheckIn(currentLocationLatitude,currentLocationLongitude);
            messageText.text = result;
        }

        private async void JoinRace()
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                await CheatCode.Instance.JoinRaceRequest();
                return;
            }
            string responseData = await GameManager.Instance.CloudCode.JoinRace(currentLocationLatitude, currentLocationLongitude);

            if (!string.IsNullOrEmpty(responseData))
            {
                UGS.CloudCode.JoinRaceResponse joinRaceResponse = JsonConvert.DeserializeObject<UGS.CloudCode.JoinRaceResponse>(responseData);
                DateTime setCurrentDateTime = CheatCode.Instance.IsCheatEnabled ? DateTime.UtcNow : DateTime.UtcNow;
                DateTime raceTime = DateTime.ParseExact(joinRaceResponse.RaceTime, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);

                GameManager.Instance.GameData.SetGameData(joinRaceResponse.CanWaitInLobby, raceTime, setCurrentDateTime);
                if (joinRaceResponse.CanWaitInLobby)
                {
                    // Show the lobby screen
                }
                else
                {
                    messageText.text = joinRaceResponse.RaceTime;
                }
                OpenTab(ScreenTabType.LobbyWait);
            }
            else
            {
                messageText.text = responseData;
            }
        }
    }
}
