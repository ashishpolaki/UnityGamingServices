using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class CharacterCustomisationScreen : BaseScreen
    {
        [SerializeField] private Button signOutBtn;

        #region Unity Methods
        private void OnEnable()
        {
            signOutBtn.onClick.AddListener(() => SignOut());
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Authentication.OnSignedOut += OnSignedOutEvent;
            }
            Enable();
        }
        private void OnDisable()
        {
            signOutBtn.onClick.RemoveAllListeners();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Authentication.OnSignedOut -= OnSignedOutEvent;
            }
        }
        #endregion

        #region Private Methods
        private void Enable()
        {
            //If player name is empty then open player name tab
            if (string.IsNullOrEmpty(GameManager.Instance.GameData.PlayerName))
            {
                OpenTab(ScreenTabType.PlayerName);
            }
            else
            {
                //Open Role Selection Tab
                OpenTab(ScreenTabType.RoleSelection);
            }
        }
        private void SignOut()
        {
            GameManager.Instance.Authentication.Signout();
        }
        private void OnSignedOutEvent()
        {
            GameManager.Instance.ResetData();
            UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
            Close();
        }
        #endregion
    }
}
