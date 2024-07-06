using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;   // Reference to the Host button in the UI
    public Button clientButton; // Reference to the Connect as Client button in the UI
    public Button startServer;

    [SerializeField] private TMP_InputField ipAddressInput;
    [SerializeField] private TMP_InputField portInput;

    void Start()
    {
        // Add listeners to the buttons
        hostButton.onClick.AddListener(Host);
        clientButton.onClick.AddListener(ConnectAsClient);
        startServer.onClick.AddListener(StartServer);
    }

    void Host()
    {
        // Start hosting a server
        NetworkManager.Singleton.StartHost();
    }

    void ConnectAsClient()
    {
        // Connect as a client to a server
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddressInput.text, ushort.Parse(portInput.text));
        NetworkManager.Singleton.StartClient();

    }

    void StartServer()
    {
        // Stop the server if hosting
        NetworkManager.Singleton.StartServer();
    }
    
}