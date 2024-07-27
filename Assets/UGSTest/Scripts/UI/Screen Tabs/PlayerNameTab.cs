using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class PlayerNameTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private Button setPlayerNameBtn;
        [SerializeField] private Button generateRandomNameBtn;
        [SerializeField] private TextMeshProUGUI errorMessageText;
        #endregion

        #region Private Variables
        private int playerNameMaxCharacters = 50;
        #endregion

        #region Unity Methods
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
        #endregion

        #region Private Methods
        private async void SetPlayerName()
        {
            errorMessageText.text = string.Empty;

            //PlayerName Criteria
            string playerName = playerNameInput.text;
            if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrEmpty(playerName) || playerName.Length > playerNameMaxCharacters || playerName.Contains(" "))
            {
                errorMessageText.text = "Player name should not be more than 50 characters and must not contain spaces.";
                return;
            }
            Func<Task> method = () => UGSManager.Instance.Authentication.SetPlayerNameAsync(playerName);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }
        private async void GenerateRandomPlayerName()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.GenerateRandomPlayerName();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Show, ScreenTabType.RoleSelection);
        }
        #endregion
    }
}