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
            Debug.LogError("--- GameManager GameObject not found. Make sure it exists in the scene. ---");
        }
        if (IsServer){
            Invoke("DestroyFireballServerRpc", 5);
        }
    }

    public void SetPlayerWhoFired(ulong playerId){
        playerOwnerId = playerId;
    }

    // [Rpc(SendTo.Server)]
    // private void DestroyFireballRpc(){
    //     NetworkObject.Despawn();
    //     Destroy(gameObject);
    // }

    [ServerRpc]
    private void DestroyFireballServerRpc(){
        NetworkObject.Despawn();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision){
        if (IsServer)
        {   
            Debug.Log("Collision");
            NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
            if (networkObject != null){
                if(collision.gameObject.CompareTag("projectile")){
                    DestroyFireballServerRpc();
                }
                else if (playerOwnerId != networkObject.OwnerClientId){
                    DestroyFireballServerRpc();
                    if(collision.gameObject.CompareTag("Player")){
                        gameManager.GetComponent<HealthManager>().applyFireballDamage(networkObject.OwnerClientId);
                    }
                }
            }
            else{
                DestroyFireballServerRpc();
            }

        }
    }

    

    
}
