using UGS;
using UnityEngine;

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

    #region Properties
    public PlayerLoginData PlayerLoginData { get; private set; }
    public Authentication Authentication { get; private set; }
    public CloudCode CloudCode { get; private set; }
    public GPS GPS { get; private set; }
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
        Authentication.OnSignedInEvent += SavePlayerLoginData;
        Authentication.OnPlayerNameChanged += HandlePlayerNameChangeEvent;
    }
    private void OnDisable()
    {
        Authentication.OnSignedInEvent -= SavePlayerLoginData;
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
        PlayerLoginData = new PlayerLoginData();
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

    #region Public Methods
    //Save Player Data on player login
    private void SavePlayerLoginData()
    {
        PlayerLoginData.SetPlayerData(Authentication);
    }
    private void HandlePlayerNameChangeEvent(string _playerName)
    {
        PlayerLoginData.SetPlayerData(playerName : _playerName);
    } 
    #endregion

}
[System.Serializable]
public class PlayerLoginData
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

    public static string SerializePlayerData(PlayerLoginData data)
    {
        return JsonUtility.ToJson(data);
    }

    public static PlayerLoginData DeserializePlayerData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerLoginData>(jsonData);
    }
}

