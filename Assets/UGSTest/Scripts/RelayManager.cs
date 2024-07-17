using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(13);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }
}