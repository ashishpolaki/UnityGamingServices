using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LoginTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button loginBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            loginBtn.onClick.AddListener(() => Login());
            GameManager.Instance.Authentication.OnSignInFailed += OnSignInFailed;
        }
        private void OnDisable()
        {
            loginBtn.onClick.RemoveAllListeners();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Authentication.OnSignInFailed -= OnSignInFailed;
            }
        }
        #endregion

        #region Private Methods
        private void OnSignInFailed(string message)
        {
            errorMessageTxt.text = message;
        }
        private async void Login()
        {
            if (string.IsNullOrEmpty(username_Input.text) || string.IsNullOrEmpty(password_Input.text))
            {
                errorMessageTxt.text = "Please fill all the fields";
                return;
            }
            Func<Task> method = () => GameManager.Instance.Authentication.SignInWithUsernamePasswordAsync(username_Input.text, password_Input.text);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion

    }
}
