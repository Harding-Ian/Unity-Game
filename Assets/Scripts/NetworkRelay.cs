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

    public static NetworkRelay Instance { get; private set; }
	    
	private void Awake() {
        Instance = this;
    }

    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();


        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //CreateRelay();
    }


    public async void CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("join code: " + joinCode); 

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        } catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

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
