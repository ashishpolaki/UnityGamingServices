using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class RegisterVenueScreen : BaseScreen
    {
        [SerializeField] private InputField locationLatitude;
        [SerializeField] private InputField locationLongitude;
        [SerializeField] private InputField radiusInput;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button fetchCurrentLocationBtn;

        private void OnEnable()
        {
            registerVenueBtn.onClick.AddListener(() => RegisterVenue());
            fetchCurrentLocationBtn.onClick.AddListener(() => FetchCurrentLocation());
        }


        private void OnDisable()
        {
            registerVenueBtn.onClick.RemoveAllListeners();
            fetchCurrentLocationBtn.onClick.RemoveAllListeners();
        }

        private async void FetchCurrentLocation()
        {
            bool result = await GameManager.Instance.GPS.TryGetLocationAsync();
            if (result)
            {
                locationLatitude.text = GameManager.Instance.GPS.CurrentLocationLatitude.ToString();
                locationLongitude.text = GameManager.Instance.GPS.CurrentLocationLongitude.ToString();
            }
        }

        private void RegisterVenue()
        {
            if (string.IsNullOrEmpty(locationLatitude.text) || string.IsNullOrEmpty(locationLongitude.text) || string.IsNullOrEmpty(radiusInput.text))
            {
                Debug.Log("Please fill all the fields");
                //     return;
            }

            float latitude = float.Parse(locationLatitude.text);
            float longitude = float.Parse(locationLongitude.text);
            float radius = float.Parse(radiusInput.text);

            UGS.CloudCode.RegisterHostItem registerHostItem = new UGS.CloudCode.RegisterHostItem
            {
                PlayerID = GameManager.Instance.PlayerLoginData.PlayerID,
                Longitude = 17.48485443046742f,
                Latitude = 78.41473168574562f,
                Radius = radius
            };
            GameManager.Instance.CloudCode.RegisterVenue(registerHostItem);

            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Show);
        }
    }
}
