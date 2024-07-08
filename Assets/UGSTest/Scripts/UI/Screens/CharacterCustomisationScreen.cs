using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button joinBtn;
        [SerializeField] private TextMeshProUGUI playerNameTxt;

        private void Awake()
        {
            hostBtn.onClick.AddListener(() => HostGame());
            joinBtn.onClick.AddListener(() => JoinGame());
        }
        private void OnDestroy()
        {
            hostBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.RemoveAllListeners();
        }
        private void Start()
        {
           SetPlayerName();
        }

        private async void SetPlayerName()
        {
            string playerName = await GameManager.Instance.Authentication.GetPlayerNameAsync();
            playerNameTxt.text = "Player Name : " + playerName;
        }

        private void JoinGame()
        {
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Close);

        }
        private void HostGame()
        {
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Close);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Open);
        }
    }
}
