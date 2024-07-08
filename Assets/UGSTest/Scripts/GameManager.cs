using System.Collections;
using System.Collections.Generic;
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
    public UGS.Authentication Authentication { get; private set; }
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

        GPS = new GPS();
        InitializeUGS();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        Authentication.DeSubscribeEvents();
    }
    #endregion

    /// <summary>
    /// Initialize Unity Gaming Services
    /// </summary>
    private void InitializeUGS()
    {
        Authentication = new UGS.Authentication();
        Authentication.InitializeUnityServices();
    }
}
