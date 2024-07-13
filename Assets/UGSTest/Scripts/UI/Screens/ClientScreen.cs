using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screen
{
    public class ClientScreen : BaseScreen
    {
        [SerializeField] private Button checkedInBtn;
        [SerializeField] private TextMeshProUGUI messageText;

        [SerializeField] private double currentLocationLatitude;
        [SerializeField] private double currentLocationLongitude;

        private void OnEnable()
        {
            checkedInBtn.onClick.AddListener(() => CheckIn());
        }
        private void OnDisable()
        {
            checkedInBtn.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            FetchCurrentLocation();
        }

        private async void FetchCurrentLocation()
        {
            bool result = await GameManager.Instance.GPS.TryGetLocationAsync();
            if (result)
            {
                currentLocationLatitude = GameManager.Instance.GPS.CurrentLocationLatitude;
                currentLocationLongitude = GameManager.Instance.GPS.CurrentLocationLongitude;
            }
            currentLocationLatitude = 17.48477376610915;
            currentLocationLongitude = 78.41440387735862;
        }
        private async void CheckIn()
        {
            var checkInRequest = new UGS.CloudCode.CheckInRequest
            {
                PlayerID = GameManager.Instance.PlayerLoginData.PlayerID,
                Latitude = currentLocationLatitude,
                Longitude = currentLocationLongitude
            };
            var result = await GameManager.Instance.CloudCode.CheckIn(checkInRequest);
            messageText.text = result;
        }
    }
}
