using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LobbyWaitTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI raceStartTimeTxt;
        [SerializeField] private Button raceCheckInBtn;

        private TimeSpan timeLeft = new TimeSpan();
        private double currentLocationLatitude;
        private double currentLocationLongitude;

        private void OnEnable()
        {
            raceCheckInBtn.onClick.AddListener(() => RaceConfirmCheckIn());
        }
        private void OnDisable()
        {
            raceCheckInBtn.onClick.RemoveAllListeners();
        }


        private void Start()
        {
            currentLocationLatitude = GameManager.Instance.GPS.CurrentLocationLatitude;
            currentLocationLongitude = GameManager.Instance.GPS.CurrentLocationLongitude;

            if (CheatCode.Instance.IsCheatEnabled)
            {
                DateTime currentDateTime = DateTime.UtcNow.Add(DateTime.Parse(CheatCode.Instance.CheatDateTime) - DateTime.UtcNow);
                timeLeft = GameManager.Instance.GameData.RaceTime - currentDateTime;
            }
            StartCoroutine(StartCountDownTimeLeft());
        }

        IEnumerator StartCountDownTimeLeft()
        {
            while (timeLeft.TotalSeconds >= 0)
            {
                raceStartTimeTxt.text = $"Race Starts in : \n {timeLeft.Hours.ToString("D2")}:{timeLeft.Minutes.ToString("D2")}:{timeLeft.Seconds.ToString("D2")}";
                yield return new WaitForSecondsRealtime(1f);
                //Decrease the timeleft by 1 second
                timeLeft = timeLeft.Add(TimeSpan.FromSeconds(-1));
                //timeLeft.Add(TimeSpan.FromSeconds(-1));
            }
            raceStartTimeTxt.text = "Race Will Start Soon";
        }

        private async void RaceConfirmCheckIn()
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                currentLocationLatitude = CheatCode.Instance.Latitude;
                currentLocationLongitude = CheatCode.Instance.Longitude;
            }
            string message = await GameManager.Instance.CloudCode.ConfirmRaceCheckIn(currentLocationLatitude, currentLocationLongitude);
            Debug.Log(message);
        }
    }
}