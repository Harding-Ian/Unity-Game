using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class StatsManager : NetworkBehaviour
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

    public void ApplyDamage(ulong clientId){
        int fireballDamage = 2; //Base fireball damage * player modifier from player stats
        UpdateHealthServerRpc(fireballDamage, clientId);
    }

    public void UpdateKnockback(ulong clientId, float knockback){
        UpdateKnockbackServerRpc(clientId, knockback);
    }

    [ServerRpc]
    private void UpdateHealthServerRpc(int damage, ulong clientId){
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<PlayerStatsManager>().playerHealth.Value -= damage;
        if (networkObject.GetComponent<PlayerStatsManager>().playerHealth.Value <= 0){
            //apply death
            Debug.Log("Player " + clientId + " died");
            
            // networkObject.GetComponent<MouseLook>().ToggleSpectateOn();
        }
    }

    [ServerRpc]
    private void UpdateKnockbackServerRpc(ulong clientId, float knockback){
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value += knockback;
    }

}