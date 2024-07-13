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

        private async void RegisterVenue()
        {
            if (string.IsNullOrEmpty(locationLatitude.text) || string.IsNullOrEmpty(locationLongitude.text) || string.IsNullOrEmpty(radiusInput.text))
            {
                Debug.Log("Please fill all the fields");
                 return;
            }

            double latitude = double.Parse(locationLatitude.text);
            double longitude = double.Parse(locationLongitude.text);
            latitude = 17.48477376610915;
            longitude = 78.41440387735862;
            float radius = float.Parse(radiusInput.text);

            UGS.CloudCode.RegisterHostItem registerHostItem = new UGS.CloudCode.RegisterHostItem
            {
                PlayerID = GameManager.Instance.PlayerLoginData.PlayerID,
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius
            };
            await GameManager.Instance.CloudCode.RegisterVenue(registerHostItem);

            UIController.Instance.ScreenEvent(ScreenType, UIScreenEvent.Hide);
            UIController.Instance.ScreenEvent(ScreenType.Host, UIScreenEvent.Show);
        }
    }
}
