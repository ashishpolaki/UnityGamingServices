using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RoleSelectionTab : BaseTab
    {
        #region Inspector variables
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button joinBtn;
        [SerializeField] private TextMeshProUGUI playerNameTxt;
        #endregion

        #region Unity methods
        private void OnEnable()
        {
            hostBtn.onClick.AddListener(() => HostGame());
            joinBtn.onClick.AddListener(() => JoinGame());
            SetPlayerName();
        }
        private void OnDisable()
        {
            hostBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
        private void SetPlayerName()
        {
            playerNameTxt.text = "Player Name : " + GameManager.Instance.GameData.PlayerName;
        }
        private void JoinGame()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
        }
        private void HostGame()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
        }
        #endregion
    }
}