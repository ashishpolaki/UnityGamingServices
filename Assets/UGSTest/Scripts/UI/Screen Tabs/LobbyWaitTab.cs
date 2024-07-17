using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LobbyWaitTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI raceStartTimeTxt;
        [SerializeField] private Button raceCheckInBtn;

        private void OnEnable()
        {
            raceCheckInBtn.onClick.AddListener(() => RaceConfirmCheckIn());
        }
        private void OnDisable()
        {
            raceCheckInBtn.onClick.RemoveAllListeners();
        }
        private void Update()
        {
            TimeSpan timeSpan = GameManager.Instance.GameData.RaceTime - DateTime.UtcNow;
            if (timeSpan.TotalSeconds < 0)
            {
                raceStartTimeTxt.text = "Race Will Start Soon";
            }
            else
            {
                raceStartTimeTxt.text = $"Race Starts in : \n {timeSpan.Hours.ToString("D2")}:{timeSpan.Minutes.ToString("D2")}:{timeSpan.Seconds.ToString("D2")}";
            }
        }
        private async void RaceConfirmCheckIn()
        {
            string message = await GameManager.Instance.CloudCode.ConfirmRaceCheckIn(GameManager.Instance.PlayerData.PlayerID);
            Debug.Log(message);
        }
    }
}