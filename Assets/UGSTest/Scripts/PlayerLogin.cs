using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerLogin : MonoBehaviour
{
    #region Unity Methods
    private void OnEnable()
    {
        GameManager.Instance.Authentication.OnSignedInEvent += SignInSuccessful;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Authentication.OnSignedInEvent -= SignInSuccessful;
        }
    }
    private async void Start()
    {
        if (GameManager.Instance.Authentication.IsSignInCached())
        {
            Func<Task> method = () => GameManager.Instance.Authentication.CacheSignInAsync();
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
        }
        else
        {
            UI.UIController.Instance.ScreenEvent(ScreenType.Login, UIScreenEvent.Open);
        }
        GameManager.Instance.GPS.RequestPermission();
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
