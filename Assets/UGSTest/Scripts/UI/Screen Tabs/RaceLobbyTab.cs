using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RaceLobbyTab : BaseTab
    {
        [SerializeField] private Button startRace_btn;
        [SerializeField] private TextMeshProUGUI checkInPlayerNamesTxt;

        private List<string> playerNames = new List<string>();
        private async void Start()
        {
            //Check in players
            string response = await GameManager.Instance.CloudCode.GetLobbyPlayers();
            playerNames = JsonConvert.DeserializeObject<List<string>>(response);
            if (playerNames.Count > 0)
            {
                checkInPlayerNamesTxt.text = string.Join("\n", playerNames);
            }
            //Enable button if more than 2 players are checked in.
            //startRace_btn.interactable = (playerNames.Count > 2);
        }
        private void OnEnable()
        {
            startRace_btn.onClick.AddListener(() => StartRace());
        }
        private void OnDisable()
        {
            startRace_btn.onClick.RemoveAllListeners();
        }
        private async void StartRace()
        {
            string horsesInRaceOrder = JsonConvert.SerializeObject(GameManager.Instance.horsesInRaceOrderList);
            await GameManager.Instance.CloudCode.StartRace(horsesInRaceOrder);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Show, ScreenTabType.RaceInProgress);
            gameObject.SetActive(false);
        }
    }
}
