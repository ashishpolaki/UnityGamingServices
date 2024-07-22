using System;
using System.Collections.Generic;
using UnityEngine;
using UGS;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance != null)
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    [SerializeField] private List<int> horsesInRaceOrderList = new List<int>();

    #region Properties
    public InGameData GameData { get; private set; }
    public GPS GPS { get; private set; }

    //UGS
    public Authentication Authentication { get; private set; }
    public CloudCode CloudCode { get; private set; }
    public CloudSave CloudSave { get; private set; }

    public List<int> HorsesInRaceOrderList { get => horsesInRaceOrderList; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Initialize();
        InitializeUGS();
    }
    private void OnEnable()
    {
        Authentication.OnSignedInEvent += LoginSuccessful;
        Authentication.OnPlayerNameChanged += HandlePlayerNameChangeEvent;
    }
    private void OnDisable()
    {
        Authentication.OnSignedInEvent -= LoginSuccessful;
        Authentication.OnPlayerNameChanged -= HandlePlayerNameChangeEvent;
        Authentication.DeSubscribeEvents();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initialize all the required classes
    /// </summary>
    private void Initialize()
    {
        GameData = new InGameData();
        GPS = new GPS();
    }
    /// <summary>
    /// Initialize Unity Gaming Services
    /// </summary>
    private void InitializeUGS()
    {
        Authentication = new Authentication();
        CloudCode = new CloudCode();
        CloudSave = new CloudSave();
    }
    #endregion

    private void LoginSuccessful()
    {
        CloudCode.SubscribeToPlayerMessages();
        GameData.PlayerName = Authentication.PlayerName;
        GameData.PlayerID = Authentication.PlayerID;
    }
    private void HandlePlayerNameChangeEvent(string _playerName)
    {
        GameData.PlayerName = _playerName;
    }

    public async void FetchCurrentLocation()
    {
        bool result = await GPS.TryGetLocationAsync();
        if (result)
        {
            GameData.CurrentLocationLatitude = GPS.CurrentLocationLatitude;
            GameData.CurrentLocationLongitude = GPS.CurrentLocationLongitude;
        }
    }

    public async Task<string> GetHostID()
    {
        if (string.IsNullOrEmpty(GameData.HostID))
        {
            Func<Task<string>> hostIDResponse = () => CloudSave.GetHostID(StringUtils.HOSTVENUE, GameData.CurrentLocationLatitude, GameData.CurrentLocationLongitude);
            string hostID = await LoadingScreen.Instance.PerformAsyncWithLoading(hostIDResponse);
            GameData.SetHostID(hostID);
            return hostID;
        }
        else
        {
            return GameData.HostID;
        }
    }

    public async Task<bool> IsPlayerAlreadyCheckIn()
    {
        Func<Task<bool>> response = () => CloudSave.IsPlayerAlreadyCheckIn(GameData.HostID, GameData.PlayerID, StringUtils.RACECHECKIN);
        bool isChecked = await LoadingScreen.Instance.PerformAsyncWithLoading(response);
        return isChecked;
    }

    public async Task<string> TryGetRaceLobbyData()
    {
        Func<Task<string>> response = () => CloudSave.TryGetRaceLobbyData(GameData.HostID, GameData.PlayerID, StringUtils.RACELOBBY);
        string raceLobbyData = await LoadingScreen.Instance.PerformAsyncWithLoading(response);
        return raceLobbyData;
    }

    public async Task<string> GetRaceCheckInParticipants()
    {
        Func<Task<string>> response = () => CloudSave.GetRaceCheckInParticipants(GameData.PlayerID, StringUtils.RACECHECKIN);
        string data = await LoadingScreen.Instance.PerformAsyncWithLoading(response);
        return data;
    }

    public void ResetData()
    {
        GameData = new InGameData();
    }
}

[System.Serializable]
public class InGameData
{
    public string PlayerID { get; set; }
    public string PlayerName { get; set; }

    public DateTime RaceTime { get; set; }
    public bool CanWaitInLobby { get; set; }
    public int HorseNumber { get; set; }
    public int WinnerHorseNumber { get; set; }
    public string HostID { get; set; }
    public double CurrentLocationLatitude
    {
        get
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                return CheatCode.Instance.Latitude;
            }
            currentLocationLatitude = 17.48477376610915;
            return currentLocationLatitude;
        }
        set
        {
            currentLocationLatitude = value;
        }
    }
    public double CurrentLocationLongitude
    {
        get
        {
            if (CheatCode.Instance.IsCheatEnabled)
            {
                return CheatCode.Instance.Longitude;
            }
            currentLocationLongitude = 78.41440387735862;
            return currentLocationLongitude;
        }
        set
        {
            currentLocationLongitude = value;
        }
    }

    private double currentLocationLatitude;
    private double currentLocationLongitude;

    public bool IsRaceWinner()
    {
        return WinnerHorseNumber == HorseNumber;
    }
    public void SetHostID(string _hostID)
    {
        HostID = _hostID;
    }
}

