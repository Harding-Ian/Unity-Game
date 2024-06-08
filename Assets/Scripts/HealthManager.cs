using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class HealthManager : NetworkBehaviour
{
    // Dictionary to store health for each player by clientId
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

    public void applyDamage(ulong clientId){
        int fireballDamage = 2; //Base fireball damage * player modifier from player stats
        updateHealthServerRpc(fireballDamage, clientId);
    }

    [ServerRpc]
    private void updateHealthServerRpc(int damage, ulong clientId){
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<PlayerStatsManager>().playerHealth.Value -= damage;
        networkObject.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value += 0.1f;
        if (networkObject.GetComponent<PlayerStatsManager>().playerHealth.Value <= 0){
            //apply death
            Debug.Log("Player " + clientId + " died");
            
            // networkObject.GetComponent<MouseLook>().ToggleSpectateOn();
        }
    }

}