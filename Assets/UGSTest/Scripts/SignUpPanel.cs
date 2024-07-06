using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;


public class SignUpPanel : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button signUpBtn;
    public Button signInBtn;
    public Button signOutBtn;
    public Button signInAnonymousBtn;
    public TextMeshProUGUI playerNameTxt;
    public TextMeshProUGUI deviceIDtxt;
    public TextMeshProUGUI messageTxt;
    public SaveManager saveManager;

    private void Awake()
    {
        DeviceID();
    }
    private void OnDisable()
    {
        DeSubscribeEvents();
    }
   
    private void DeviceID()
    {
        deviceIDtxt.text = SystemInfo.deviceUniqueIdentifier.ToString();
    }

    #region Events
    private void SubscribeEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            messageTxt.text = "Player signed in.";
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
            messageTxt.text = err.Message;
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
            messageTxt.text = "Player signed out.";
        };

        AuthenticationService.Instance.Expired += () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
            messageTxt.text = "Player session could not be refreshed and expired.";
        };
    }

    void DeSubscribeEvents()
    {
        AuthenticationService.Instance.SignedIn -= () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed -= (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut -= () =>
        {
            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired -= () =>
        {
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
    #endregion

}
