using System;
using System.Threading.Tasks;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI messageTxt;
        [SerializeField] private float radiusMin = 0;
        [SerializeField] private float radiusMax = 100;
        #endregion

        #region Private Variables
        private double latitude;
        private double longitude;
        private float radius;
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
            messageTxt.text = string.Empty;

            if (!IsGPSLocationValid() || !IsRadiusValid())
            {
                return;
            }

            //Register Host in Venue Cloud List.
            UGS.CloudCode.HostVenueData registerHostItem = new UGS.CloudCode.HostVenueData
            {
                Latitude = latitude,
                Longitude = longitude,
                Radius = radius
            };
            Func<Task> method = () => GameManager.Instance.CloudCode.RegisterVenue(registerHostItem);
            await LoadingScreen.Instance.PerformAsyncWithLoading(method);
            Close();
        }

        private bool IsGPSLocationValid()
        {
            if (string.IsNullOrEmpty(locationLatitude.text) || string.IsNullOrEmpty(locationLongitude.text))
            {
                messageTxt.text = "Please fetch the current location";
                return false;
            }
            latitude = double.Parse(locationLatitude.text);
            longitude = double.Parse(locationLongitude.text);

            bool isGPSValid = GameManager.Instance.GPS.IsValidGpsLocation(latitude, longitude);
            if (!isGPSValid)
            {
                messageTxt.text = "The GPS location is not valid.";
                return false;
            }

            return true;
        }

        private bool IsRadiusValid()
        {
            if (string.IsNullOrEmpty(radiusInput.text))
            {
                messageTxt.text = "Please enter the radius";
                return false;
            }
            radius = float.Parse(radiusInput.text);
            if (radius <= radiusMin)
            {
                messageTxt.text = "Radius should be greater than one.";
                return false;
            }
            if (radius >= radiusMax)
            {
                messageTxt.text = "Radius should be less than 100.";
                return false;
            }
            return true;
        }
        #endregion

    }
}
