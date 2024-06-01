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

    public void applyFireballDamage(ulong clientId){
        int fireballDamage = 2; //Base fireball damage * player modifier from player stats
        updateHealthServerRpc(fireballDamage, clientId);
    }

    [ServerRpc]
    private void updateHealthServerRpc(int damage, ulong clientId){
        Debug.Log("calling me splurge");
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<HealthBar>().health.Value -= damage;
        networkObject.GetComponent<HealthBar>().logHealth(clientId);
        if (networkObject.GetComponent<HealthBar>().health.Value <= 0){
            //apply death
            Debug.Log("Player " + clientId + " died");
        }
    }

}