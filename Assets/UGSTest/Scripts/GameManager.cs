using System;
using System.Collections.Generic;
using UnityEngine;
using UGS;

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
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    public List<int> horsesInRaceOrderList = new List<int>();

    #region Properties
    public PlayerData PlayerData { get; private set; }
    public InGameData GameData { get; private set; }
    public GPS GPS { get; private set; }

    //UGS
    public Authentication Authentication { get; private set; }
    public CloudCode CloudCode { get; private set; }
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
        PlayerData = new PlayerData();
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
    }
    #endregion
    private void LoginSuccessful()
    {
        CloudCode.SubscribeToPlayerMessages();
        PlayerData.SetPlayerData(Authentication);
    }
    private void HandlePlayerNameChangeEvent(string _playerName)
    {
        PlayerData.SetPlayerData(playerName: _playerName);
    }
}
[System.Serializable]
public class PlayerData
{
    public string PlayerID { get; private set; }
    public string PlayerName { get; private set; }

    public void SetPlayerData(string playerId = "", string playerName = "")
    {
        PlayerID = string.IsNullOrEmpty(playerId) ? PlayerID : playerId;
        PlayerName = string.IsNullOrEmpty(playerName) ? PlayerName : playerName;
    }
    public void SetPlayerData(Authentication authentication)
    {
        PlayerID = authentication.PlayerID;
        PlayerName = authentication.PlayerName;
    }


    public static string SerializePlayerData(PlayerData data)
    {
        return JsonUtility.ToJson(data);
    }

    public static PlayerData DeserializePlayerData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerData>(jsonData);
    }
}

[System.Serializable]
public class InGameData
{
    public DateTime RaceTime { get; private set; }
    public bool CanWaitInLobby { get; private set; }
    public DateTime CurrentTime { get; private set; }
    public int HorseNumber { get; private set; }
    public int WinnerHorseNumber { get; private set; }
    public void SetGameData(bool _CanWaitInLobby, DateTime raceTime, DateTime currentTime)
    {
        RaceTime = raceTime;
        CurrentTime = currentTime;
        CanWaitInLobby = _CanWaitInLobby;
    }
    public void SetHorseNumber(int _horseNumber)
    {
        HorseNumber = _horseNumber;
    }
    public void SetWinnerHorseNumber(int _winnerHorseNumber)
    {
        WinnerHorseNumber = _winnerHorseNumber;
    }

    public bool IsRaceWinner()
    {
        return WinnerHorseNumber == HorseNumber;
    }
}

