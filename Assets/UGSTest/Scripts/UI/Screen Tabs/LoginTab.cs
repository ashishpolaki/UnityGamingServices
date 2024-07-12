using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class LoginTab : BaseTab
    {
        [SerializeField] private InputField username_Input;
        [SerializeField] private InputField password_Input;
        [SerializeField] private Button loginBtn;
        [SerializeField] private TextMeshProUGUI errorMessageTxt;

        private void OnEnable()
        {
            loginBtn.onClick.AddListener(() => Login());
            GameManager.Instance.Authentication.OnSignInFailed += OnSignInFailed;

        }
        private void OnDisable()
        {
            loginBtn.onClick.RemoveAllListeners();
            GameManager.Instance.Authentication.OnSignInFailed -= OnSignInFailed;
        }
        private void OnSignInFailed(string message)
        {
            errorMessageTxt.text = message;
        }
        private void Login()
        {
            if(string.IsNullOrEmpty(username_Input.text) || string.IsNullOrEmpty(password_Input.text))
            {
                errorMessageTxt.text = "Please fill all the fields";
                return;
            }
            GameManager.Instance.Authentication.SignInWithUsernamePasswordAsync(username_Input.text, password_Input.text);
        }
    }
}
