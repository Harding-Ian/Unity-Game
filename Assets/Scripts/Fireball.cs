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
    }

    // private void OnCollisionEnter(Collision collision){
    //     if (IsServer)
    //     {   
    //         Debug.Log("Collision");
    //         NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
    //         if (networkObject != null){
    //             if(collision.gameObject.CompareTag("projectile")){
    //                 DestroyFireballServerRpc();
    //             }
    //             else if (playerOwnerId != networkObject.OwnerClientId){
    //                 DestroyFireballServerRpc();
    //                 if(collision.gameObject.CompareTag("Player")){
    //                     gameManager.GetComponent<HealthManager>().applyFireballDamage(networkObject.OwnerClientId);
    //                 }
    //             }
    //         }
    //         else{
    //             DestroyFireballServerRpc();
    //         }

    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
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
