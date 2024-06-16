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

    public void ApplyDamage(ulong damagedPlayerId, float damage, ulong damagingPlayerId){
        UpdateHealthServerRpc(damage, damagedPlayerId, damagingPlayerId);
    }

    public void UpdateKnockback(ulong clientId, float knockback)
    {
        UpdateKnockbackServerRpc(clientId, knockback);
    }

    [ServerRpc]
    private void UpdateHealthServerRpc(float damage, ulong damagedPlayerId, ulong damagingPlayerId)
    {
        NetworkObject damagedPlayer = NetworkManager.Singleton.ConnectedClients[damagedPlayerId].PlayerObject;

        damagedPlayer.GetComponent<PlayerStatsManager>().playerHealth.Value -= damage;

        if(damagingPlayerId != damagedPlayerId) 
        {
            damagedPlayer.GetComponent<PlayerScript>().lastDamagingPlayerId.Value = damagingPlayerId;
        }

        if (damagedPlayer.GetComponent<PlayerStatsManager>().playerHealth.Value <= 0)
        {
            damagedPlayer.GetComponent<PlayerDeath>().InitiatePlayerDeath();
        }
        
    }

    [ServerRpc]
    private void UpdateKnockbackServerRpc(ulong clientId, float knockback){
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value += knockback;
    }

}