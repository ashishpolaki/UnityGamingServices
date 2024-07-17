using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class HostRaceInProgressTab : BaseTab
    {
        [SerializeField] private Button raceResultsButton;

        private void OnEnable()
        {
            raceResultsButton.onClick.AddListener(() => ShowRaceResults());
        }
        private void OnDisable()
        {
            raceResultsButton.onClick.RemoveAllListeners();
        }
        private async void ShowRaceResults()
        {
            int winnerHorseNumber = GameManager.Instance.horsesInRaceOrderList[0];
            await GameManager.Instance.CloudCode.SendRaceResults(winnerHorseNumber);
        }
    }
}