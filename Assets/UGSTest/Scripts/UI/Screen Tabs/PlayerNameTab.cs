using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerNameTab : BaseTab
    {
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private Button setPlayerNameBtn;
        [SerializeField] private Button generateRandomNameBtn;
        [SerializeField] private TextMeshProUGUI errorMessageText;

        //playername Validation Criteria
        private int playerNameMaxCharacters = 50;

        private void OnEnable()
        {
            setPlayerNameBtn.onClick.AddListener(() => SetPlayerName());
            generateRandomNameBtn.onClick.AddListener(() => GenerateRandomPlayerName());
        }

        private void OnDisable()
        {
            setPlayerNameBtn.onClick.RemoveAllListeners();
            generateRandomNameBtn.onClick.RemoveAllListeners();
        }

        private async void SetPlayerName()
        {
            //PlayerName Criteria
            string playerName = playerNameInput.text;
            if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrEmpty(playerName) || playerName.Length > playerNameMaxCharacters)
            {
                errorMessageText.text = "Player name should not be more than 50 characters and must not contain spaces.";
                return;
            }
            await GameManager.Instance.Authentication.SetPlayerNameAsync(playerName);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }

        private async void GenerateRandomPlayerName()
        {
            await GameManager.Instance.Authentication.GenerateRandomPlayerName();
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }
    }
}