using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RegisterTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button registerPlayer_btn;
        [SerializeField] private TextMeshProUGUI errorMessage_txt;
        #endregion

        #region Private Variables
        //username validation Criteria
        private Regex usernameCharacters = new Regex(@".{3,20}");
        private string usernameErrorMessage = "Username must be between 3 and 20 characters";

        // Password validation criteria
        private Regex hasMinimumChars = new Regex(@".{8,30}");
        private Regex hasUpperCaseLetter = new Regex(@"[A-Z]+");
        private Regex hasLowerCaseLetter = new Regex(@"[a-z]+");
        private Regex hasDecimalDigit = new Regex(@"[0-9]+");
        private Regex hasSymbol = new Regex(@"[\W]+");
        private string passwordErrorMessage = "Password must be 8-30 characters long, with at least 1 uppercase,1 lowercase,1 number,1 symbol.";
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            registerPlayer_btn.onClick.AddListener(() => RegisterPlayer());
            GameManager.Instance.Authentication.OnSignInFailed += OnSignUpFailed;
        }
        private void OnDisable()
        {
            registerPlayer_btn.onClick.RemoveAllListeners();
            GameManager.Instance.Authentication.OnSignInFailed -= OnSignUpFailed;
        }
        #endregion

        #region Unity Methods
        //signup failed method
        private void OnSignUpFailed(string message)
        {
            errorMessage_txt.text = message;
        }
        private async void RegisterPlayer()
        {
            //Username Criteria
            string userName = username_Input.text;
            if (!usernameCharacters.IsMatch(userName))
            {
                errorMessage_txt.text = usernameErrorMessage;
                return;
            }

            //Password Criteria
            string password = password_Input.text;
            if (!hasMinimumChars.IsMatch(password) ||
             !hasUpperCaseLetter.IsMatch(password) ||
             !hasLowerCaseLetter.IsMatch(password) ||
              !hasDecimalDigit.IsMatch(password) ||
             !hasSymbol.IsMatch(password))
            {
                errorMessage_txt.text = passwordErrorMessage;
                return;
            }

            Func<Task> method = () => GameManager.Instance.Authentication.SignUpAsync(userName, password);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        #endregion
    }
}
