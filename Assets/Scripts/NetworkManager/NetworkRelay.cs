using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class NetworkRelay : MonoBehaviour
{
    // Start is called before the first frame update

    public bool AllowConnections = true;

    public async void CreateRelayAndGameMenu(UISceneManager uISceneManager) {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("join code: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        } catch (RelayServiceException e){
            Debug.Log(e);
            uISceneManager.hostButtonClickable = true;
        }
        uISceneManager.LoadGameMenu();
    }

    public async void JoinRelay(string joinCode, UISceneManager uISceneManager){
        Debug.Log("-------------------------------------------------------------------------------------------------------------------------------");
        Debug.Log("code received: " + joinCode);
        string editedJoinCode = joinCode.Replace(" ", string.Empty);
        // return;
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(editedJoinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

            uISceneManager.UnloadNetworkMenu();

        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

    

}
