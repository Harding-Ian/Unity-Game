using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
    
    private ulong playerOwnerId;

    public GameObject gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found within scene");
        }
        if (IsServer){
            Invoke("DestroyFireball", 5);
        }
    }

    public void SetPlayerWhoFired(ulong playerId){
        playerOwnerId = playerId;
    }

    [Rpc(SendTo.Everyone)]
    private void FireballRpc(ulong clientId){
        // Debug.Log("fireballrpc run");
        // Vector3 knockbackdirection;
        // knockbackdirection = new Vector3(0,1,0);
        // NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        // networkObject.GetComponent<PlayerMovement>().ApplyKnockback(knockbackdirection, 10);
    }

    private void DestroyFireball()
    {
        NetworkObject.Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();

        if(IsClient) Debug.Log("IsLocalPlayer is true");
        if(networkObject != null) Debug.Log("networkObject != null is true");
        if(playerOwnerId != networkObject.OwnerClientId) Debug.Log("playerOwnerId != networkObject.OwnerClientId is true");
        if(other.gameObject.CompareTag("Player")) Debug.Log("other.gameObject.CompareTag is true");
        

        if(IsClient && networkObject != null && playerOwnerId != networkObject.OwnerClientId && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("fireballrpc run");
            Vector3 knockbackdirection;
            knockbackdirection = new Vector3(0,1,0);
            networkObject.GetComponent<PlayerMovement>().ApplyKnockback(knockbackdirection, 100);
        }

        if (IsServer)
        {
            if (networkObject != null)
            {
                if (other.gameObject.CompareTag("projectile"))
                {
                    //DestroyFireballServerRpc();
                    NetworkObject.Despawn();
                }
                else if (playerOwnerId != networkObject.OwnerClientId)
                {
                    //DestroyFireballServerRpc();
                    NetworkObject.Despawn();
                    if (other.gameObject.CompareTag("Player"))
                    {
                        gameManager.GetComponent<HealthManager>().applyFireballDamage(networkObject.OwnerClientId);
                    }
                }
            }
            else
            {
                //DestroyFireballServerRpc();
                NetworkObject.Despawn();
            }
        }


    }

    

    
}
