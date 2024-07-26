using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerLobbyTab : BaseTab
    {
        [SerializeField] private TextMeshProUGUI raceStartTimeTxt;
        [SerializeField] private Button raceCheckInBtn;

        private TimeSpan timeLeft = new TimeSpan();
        private Coroutine coroutine;
        private void OnEnable()
        {
            raceCheckInBtn.onClick.AddListener(() => RaceConfirmCheckIn());
            PlayerLobbyStatus();
        }
        private void OnDisable()
        {
            raceCheckInBtn.onClick.RemoveAllListeners();
        }
        private async void PlayerLobbyStatus()
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                DateTime currentDateTime = DateTime.UtcNow.Add(DateTime.Parse(CheatCode.Instance.CheatDateTime) - DateTime.UtcNow);
                timeLeft = GameManager.Instance.GameData.RaceTime - currentDateTime;
            }
            else
            {
                timeLeft = GameManager.Instance.GameData.RaceTime - DateTime.UtcNow;
            }
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(StartCountDownTimeLeft());

            bool isCheckedInAlready = await GameManager.Instance.IsPlayerAlreadyCheckIn();
            if (isCheckedInAlready)
            {
                raceCheckInBtn.interactable = false;
            }
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
            Func<Task<bool>> confirmCheckinResponse = () => GameManager.Instance.CloudCode.ConfirmRaceCheckIn(GameManager.Instance.GameData.HostID, GameManager.Instance.GameData.PlayerName);
            bool isCheckedIn = await LoadingScreen.Instance.PerformAsyncWithLoading(confirmCheckinResponse);
            if (isCheckedIn)
            {
                raceCheckInBtn.interactable = false;
            }
        }
    }
}