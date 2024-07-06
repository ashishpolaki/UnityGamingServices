using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    private async void Start()
    {
      //   AuthenticationService.Instance.SignOut();

        // Initialize the Unity Services Core SDK
        await UnityServices.InitializeAsync();

        // Authenticate by logging into an anonymous account
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

         SaveData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AuthenticationService.Instance.SignOut(true);
        }
    }

    public async void SaveData()
    {
        var playerData = new Dictionary<string, object>{
          {"firstKeyName", "a text value"},
          {"secondKeyName", 121}
        };
        var result = await CloudSaveService.Instance.Data.Player.SaveAsync(playerData, new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }


}
#if false
public class AuthenticationManager : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
public class RaceStateManager : MonoBehaviour
{
    private const string RaceStateKey = "RaceState";

    public async void SaveRaceState(bool isRaceReady)
    {
        Dictionary<string, object> data = new Dictionary<string, object> { { RaceStateKey, isRaceReady } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }

    public async void LoadRaceState(System.Action<bool> callback)
    {
        try
        {
            Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { RaceStateKey });
            if (savedData.ContainsKey(RaceStateKey))
            {
                bool isRaceReady = bool.Parse(savedData[RaceStateKey]);
                callback(isRaceReady);
            }
            else
            {
                callback(false);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            callback(false);
        }
    }
}
public class RelayManager : MonoBehaviour
{
    public async void CreateRelayHost()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(12);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log($"Join Code: {joinCode}");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.ConnectionData,
            allocation.Key
        );
        NetworkManager.Singleton.StartHost();

        // Save race state to indicate host is ready
        FindObjectOfType<RaceStateManager>().SaveRaceState(true);
    }
}
#endif