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
            if (GameManager.Instance != null)
            {
            GameManager.Instance.Authentication.OnSignedInEvent -= SignedIn;
            }
      
            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
            signInAnonymousBtn.onClick.RemoveAllListeners();
        }
        #endregion

        private void OpenLoginTab()
        {
            ButtonInteractable(ScreenTabType.LoginPlayer);
            CloseTab(ScreenTabType.RegisterPlayer);
            OpenTab(ScreenTabType.LoginPlayer);
        }

        private void OpenRegisterTab()
        {
            ButtonInteractable(ScreenTabType.RegisterPlayer);
            CloseTab(ScreenTabType.LoginPlayer);
            OpenTab(ScreenTabType.RegisterPlayer);
        }
        private void SignInAnonymously()
        {
            GameManager.Instance.Authentication.SignInAnonymouslyAsync();
        }
        private void ButtonInteractable(ScreenTabType screenType)
        {
            registerTabBtn.interactable = !(screenType == ScreenTabType.RegisterPlayer);
            loginTabBtn.interactable = !(screenType == ScreenTabType.LoginPlayer);
        }

        #region Subscribe Methods
        private void SignedIn()
        {
            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Close);
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
        }
        #endregion
    }
}