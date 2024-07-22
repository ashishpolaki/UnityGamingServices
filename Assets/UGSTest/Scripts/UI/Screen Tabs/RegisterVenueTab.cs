using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen.Tab
{
    public class RegisterVenueTab : BaseTab
    {
        #region Inspector Variables
        [SerializeField] private InputField locationLatitude;
        [SerializeField] private InputField locationLongitude;
        [SerializeField] private InputField radiusInput;
        [SerializeField] private Button registerVenueBtn;
        [SerializeField] private Button fetchCurrentLocationBtn;
        #endregion

        #region Unity Methods
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
        #endregion

        #region Private Methods
        private async void FetchCurrentLocation()
        {
            Func<Task<bool>> method = () => GameManager.Instance.GPS.TryGetLocationAsync();
            bool result = await LoadingScreen.Instance.PerformAsyncWithLoading<bool>(method);
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
            float radius = float.Parse(radiusInput.text);
            //latitude = 17.48477376610915;
            //longitude = 78.41440387735862;

            UGS.CloudCode.HostVenueData registerHostItem = new UGS.CloudCode.HostVenueData
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius
            };
            await GameManager.Instance.CloudCode.RegisterVenue(registerHostItem);
            Close();
        }
        #endregion

    }
}
