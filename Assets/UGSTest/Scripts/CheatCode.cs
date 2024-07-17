using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;
using UnityEngine.UI;

public class CheatCode : MonoBehaviour
{
    public static CheatCode Instance;

    [SerializeField] private Button cheatButton;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private Toggle cheatToggle;
    [SerializeField] private InputField latitudeInput;
    [SerializeField] private InputField longitudeInput;

    private bool IsCheatPanelActive;
    public bool IsCheatEnabled;

    [SerializeField] private TimeAdjustmentSettings cheatTime;

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        cheatButton.onClick.AddListener(() => CheatPanel());
        closeBtn.onClick.AddListener(() => CheatPanel());
        cheatToggle.onValueChanged.AddListener((value) => CheatToggle(value));
    }
    private void OnDisable()
    {
        cheatButton.onClick.RemoveAllListeners();
        cheatToggle.onValueChanged.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
    private void CheatPanel()
    {
        IsCheatPanelActive = !IsCheatPanelActive;
        cheatPanel.SetActive(IsCheatPanelActive);
    }
    private void CheatToggle(bool _val)
    {
        IsCheatEnabled = _val;
    }
    #endregion

    private string ConvertToUTC(string timeString)
    {
        DateTime localDateTime = DateTime.ParseExact(timeString, "hh:mm tt", CultureInfo.InvariantCulture);
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
        return utcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
    public async Task<string> CheckIn()
    {
        string dateTimeString = ConvertToUTC(cheatTime.ReturnTime());

        var checkInRequest = new UGS.CloudCode.CheckInRequest
        {
            PlayerID = GameManager.Instance.PlayerData.PlayerID,
            Latitude = !string.IsNullOrEmpty(latitudeInput.text) ? double.Parse(latitudeInput.text) : 17.48477376610915,
            Longitude = !string.IsNullOrEmpty(longitudeInput.text) ? double.Parse(longitudeInput.text) : 78.41440387735862,
        };
        try
        {
            string jsonData = JsonConvert.SerializeObject(checkInRequest);
            HorseRaceCloudCodeBindings module = new HorseRaceCloudCodeBindings(CloudCodeService.Instance);
            var result = await module.CheatVenueCheckIn(jsonData, dateTimeString);
            return result;
        }
        catch (CloudCodeException exception)
        {
            Debug.LogException(exception);
        }
        return null;
    }

    public async Task JoinRaceRequest()
    {

    }
}
