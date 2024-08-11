using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class StatsManager : NetworkBehaviour
{

    public ParticleSystem deathParticles;

    public override void OnNetworkSpawn()
    {
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

    public void ApplyDamage(ulong damagedPlayerId, float damage, ulong damagingPlayerId)
    {
        NetworkObject damagedPlayer = NetworkManager.Singleton.ConnectedClients[damagedPlayerId].PlayerObject;
        NetworkObject damagingPlayer = NetworkManager.Singleton.ConnectedClients[damagingPlayerId].PlayerObject;
        PlayerStatsManager damagingPlayerStats = damagingPlayer.GetComponent<PlayerStatsManager>();
        PlayerStatsManager damagedPlayerStats = damagedPlayer.GetComponent<PlayerStatsManager>();
        
        damagedPlayerStats.playerHealth.Value -= damage;

        if(damagingPlayerId != damagedPlayerId)
        {
            damagedPlayer.GetComponent<PlayerScript>().lastDamagingPlayerId.Value = damagingPlayerId;
        }

        if (damagedPlayer.GetComponent<PlayerStatsManager>().playerHealth.Value <= 0 && damagedPlayer.GetComponent<PlayerScript>().dead.Value == false)
        {
            damagedPlayer.GetComponent<PlayerDeath>().InitiatePlayerDeath();
            //deathAnimationRpc();
        }

        if(damagingPlayerStats.lifeSteal.Value > 0 && damagedPlayerId != damagingPlayerId)
        {
            damagingPlayerStats.playerHealth.Value += damage * damagingPlayerStats.lifeSteal.Value;

            if(damagingPlayerStats.playerHealth.Value > damagingPlayerStats.maxPlayerHealth.Value)
            {
                damagingPlayerStats.playerHealth.Value = damagingPlayerStats.maxPlayerHealth.Value;
            }
        }
    }

    
    [Rpc(SendTo.Everyone)]
    private void deathParticlesRpc(){
        deathParticles.Play();
    }

    public void UpdateKnockback(ulong PlayerId, float knockbackBuildUp)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[PlayerId].PlayerObject;
        playerObject.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value += knockbackBuildUp;
    }

}