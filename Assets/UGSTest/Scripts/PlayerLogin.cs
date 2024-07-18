using UI.Screen;
using UnityEngine;

public class PlayerLogin : MonoBehaviour
{
    [SerializeField] private ScreenType openScreenType = ScreenType.Login;

    private void OnEnable()
    {
        GameManager.Instance.Authentication.OnSignedInEvent += SignIn;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Authentication.OnSignedInEvent -= SignIn;
        }
    }

    private void SignIn()
    {
        UI.UIController.Instance.ScreenEvent(ScreenType.CharacterCustomization, UIScreenEvent.Open);
    }

    void Start()
    {
        UI.UIController.Instance.ScreenEvent(openScreenType, UIScreenEvent.Open);
        if (GameManager.Instance.Authentication.IsSignInCached())
        {
            GameManager.Instance.Authentication.CacheSignInAsync();
        }
        GameManager.Instance.GPS.RequestPermission();
    }
}
