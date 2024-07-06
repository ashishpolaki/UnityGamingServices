using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
//using Unity.Services.Multiplay;
using UnityEngine;

public class MultiplayManager : MonoBehaviour
{
    // Start is called before the first frame update

    //private IServerQueryHandler serverQueryHandler;

    //[SerializeField] private TMP_InputField ipAddressInput;
    //[SerializeField] private TMP_InputField portInput;

    //private async void Start()
    //{
    //    if (Application.platform == RuntimePlatform.LinuxServer)
    //    {
    //        Application.targetFrameRate = 60;
    //        await UnityServices.InitializeAsync();
    //        ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;
    //        serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(3, "HorseRace", "Racing", "0", "TestMap");

    //        if(serverConfig.AllocationId != string.Empty)
    //        {
    //            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", serverConfig.Port, "0.0.0.0");
    //            NetworkManager.Singleton.StartServer();
    //            await MultiplayService.Instance.ReadyServerForPlayersAsync();
    //        }
    //    }
    //}

    // Update is called once per frame
    //async void Update()
    //{
    //    if(Application.platform == RuntimePlatform.LinuxServer)
    //    {
    //        if(serverQueryHandler != null)
    //        {
    //            serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
    //            serverQueryHandler.UpdateServerCheck();
    //            await Task.Delay(100);
    //        }
    //    }
    //}

    //public void JoinToServer()
    //{
    //    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    //    transport.SetConnectionData(ipAddressInput.text, ushort.Parse(portInput.text));
    //    NetworkManager.Singleton.StartClient();
    //}
}
