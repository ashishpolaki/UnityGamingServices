using System;
using System.Threading.Tasks;
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
        private void Start()
        {
            ButtonInteractable(DefaultOpenTab);
        }
        private void OnEnable()
        {
            UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;

            registerTabBtn.onClick.AddListener(() => OpenRegisterTab());
            loginTabBtn.onClick.AddListener(() => OpenLoginTab());
            signInAnonymousBtn.onClick.AddListener(() => SignInAnonymously());
        }
        private void OnDisable()
        {
            if (UGSManager.Instance != null)
            {
                UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
            }

            registerTabBtn.onClick.RemoveAllListeners();
            loginTabBtn.onClick.RemoveAllListeners();
            signInAnonymousBtn.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private Methods
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
        private async void SignInAnonymously()
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.SignInAnonymouslyAsync();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        private void ButtonInteractable(ScreenTabType screenType)
        {
            registerTabBtn.interactable = !(screenType == ScreenTabType.RegisterPlayer);
            loginTabBtn.interactable = !(screenType == ScreenTabType.LoginPlayer);
        }
        private void SignInSuccessful()
        {
            UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
            Close();
        }
        #endregion
    }
}