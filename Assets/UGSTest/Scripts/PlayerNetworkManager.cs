using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking;
using System;
using TMPro;
using System.Runtime.CompilerServices;

public class PlayerNetworkManager : NetworkBehaviour
{
    public event Action<ulong, ConnectionStatus> OnClientConnectionNotification;

    public TextMeshProUGUI joinCodeTxt;
    public TMP_InputField joinCodeInput;

    public string joinCode;

    public enum ConnectionStatus
    {
        Connected,
        Disconnected
    }
    private void Awake()
    {
    }

    private void Update()
    {
        if(NetworkManager.IsServer)
        {
            Debug.Log("Hello mmoto");
        }

    }
    private void Start()
    {
    

        if(IsServer)
        {
            Debug.Log("Server");
        }
        if(IsHost)
        {
            Debug.Log("Host");
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;


        // Since the NetworkManager can potentially be destroyed before this component, only 
        // remove the subscriptions if that singleton still exists.
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }
    }


    private void OnClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Client Connected" + NetworkManager.Singleton.ConnectedClients);
            OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Connected);
        }
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Client Disconnected" + NetworkManager.Singleton.ConnectedClients);
            OnClientConnectionNotification?.Invoke(clientId, ConnectionStatus.Disconnected);
        }
    }
    public void SetJoinCode(string _joinCode)
    {
        joinCode = _joinCode;
        joinCodeTxt.text = joinCode;
    }

    public string GetJoinCode()
    {
        return  joinCodeInput.text.ToString();
    }

}
