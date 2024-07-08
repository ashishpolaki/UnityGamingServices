using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class LoginAuthenticationScreen : BaseScreen
    {
        #region Inspector Variables
        [SerializeField] private Button registerTabBtn;
        [SerializeField] private Button loginTabBtn;
        [SerializeField] private Button signInAnonymousBtn;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            GameManager.Instance.Authentication.OnSignedInEvent += SignedIn;
            GameManager.Instance.Authentication.OnSignInFailed += SignInFail;
            GameManager.Instance.Authentication.OnSessionExpired += SessionExpire;

            registerTabBtn.onClick.AddListener(() => OpenRegisterTab());
            loginTabBtn.onClick.AddListener(() => OpenLoginTab());
            signInAnonymousBtn.onClick.AddListener(() => SignInAnonymously());
        }
        private void Start()
        {
            ButtonInteractable(DefaultOpenTab);
        }
        private void OnDisable()
        {
            GameManager.Instance.Authentication.OnSignedInEvent -= SignedIn;
            GameManager.Instance.Authentication.OnSignInFailed -= SignInFail;
            GameManager.Instance.Authentication.OnSessionExpired -= SessionExpire;

            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
            signInAnonymousBtn.onClick.RemoveAllListeners();
        }
        #endregion

        private void OpenLoginTab()
        {
            ButtonInteractable(ScreenTabType.Login);
            CloseTab(ScreenTabType.Register);
            OpenTab(ScreenTabType.Login);
        }

        private void OpenRegisterTab()
        {
            ButtonInteractable(ScreenTabType.Register);
            CloseTab(ScreenTabType.Login);
            OpenTab(ScreenTabType.Register);
        }

        private void SignInAnonymously()
        {
            GameManager.Instance.Authentication.SignInAnonymously();
        }
        private void ButtonInteractable(ScreenTabType screenType)
        {
            registerTabBtn.interactable = !(screenType == ScreenTabType.Register);
            loginTabBtn.interactable = !(screenType == ScreenTabType.Login);
        }

        #region Subscribe Methods
        private void SignedIn()
        {
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Close);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
        }
        private void SignInFail()
        {

        }
        private void SessionExpire()
        {

        }
        #endregion
    }
}