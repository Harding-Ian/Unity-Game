using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class HealthManager : NetworkBehaviour
{
    // Dictionary to store health for each player by clientId
    private Dictionary<ulong, NetworkVariable<int>> playerHealths = new Dictionary<ulong, NetworkVariable<int>>();
    private int x = 0;

    public override void OnNetworkSpawn()
    {
        // Subscribe to the OnClientConnectedCallback and OnClientDisconnectedCallback events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }


    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("ID of the client that connected: " + clientId);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log("ID of the client that disconnected: " + clientId);
    }

    void Update(){
        if(IsServer){
            x += 1;
            if (x % 1200 == 0){
                Debug.Log("Should only be seen by host and server");
                testServerRpc();
            }
        }
    }

    [ServerRpc]
    private void testServerRpc(){
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[1].PlayerObject;
            networkObject.GetComponent<PlayerMovement>().tempname.Value += 1;
    }

}