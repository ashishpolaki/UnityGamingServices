using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerLogin : MonoBehaviour
{
    #region Unity Methods
    private void OnEnable()
    {
        UGSManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
    }
    private void OnDisable()
    {
        if (UGSManager.Instance != null)
        {
            UGSManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
        }
    }
    private async void Start()
    {
        if (UGSManager.Instance.Authentication.IsSignInCached())
        {
            Func<Task> method = () => UGSManager.Instance.Authentication.CacheSignInAsync();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        else
        {
            UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
        }
        UGSManager.Instance.GPS.RequestPermission();
    }
    #endregion

    /// <summary>
    /// Sign in successful event
    /// </summary>
    private void SignInSuccessful()
    {
        UI.UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
    }
}
