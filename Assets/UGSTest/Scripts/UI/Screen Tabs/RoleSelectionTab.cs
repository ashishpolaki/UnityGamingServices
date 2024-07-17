using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RoleSelectionTab : BaseTab
    {
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button joinBtn;
        [SerializeField] private TextMeshProUGUI playerNameTxt;

        private void OnEnable()
        {
            hostBtn.onClick.AddListener(() => HostGame());
            joinBtn.onClick.AddListener(() => JoinGame());
        }
        private void OnDisable()
        {
            hostBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.RemoveAllListeners();
        }
        private void Start()
        {
            SetPlayerName();
        }
        private void SetPlayerName()
        {
            playerNameTxt.text = "Player Name : " + GameManager.Instance.PlayerData.PlayerName;
        }
        private void JoinGame()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Close);
            UIController.Instance.ScreenEvent(ScreenType.Client, UIScreenEvent.Open);
        }
        private void HostGame()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Close);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
        }
    }
}