using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
    
    private ulong playerOwnerId;
    void Start()
    {
        Invoke("DestroyFireballServerRpc", 5);
    }

    public void SetPlayerOwner(ulong playerId){
        playerOwnerId = playerId;
    }

    // Update is called once per frame
    [ServerRpc]
    private void DestroyFireballServerRpc(){
        Destroy(gameObject);
    }

    // private void OnTriggerEnter(Collider collided)
    // {
    //     if (IsServer)
    //     {   
    //         NetworkObject networkObject = collided.GetComponent<NetworkObject>();
    //         if (networkObject != null){
    //             if (playerOwnerId != networkObject.OwnerClientId){
    //                 DestroyFireballServerRpc();
    //             }
    //         }
    //         else{
    //             DestroyFireballServerRpc();
    //         }

    //     }
    // }

    private void OnCollisionEnter(Collision collision){
        if (IsServer)
        {   
            NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
            if (networkObject != null){
                if (playerOwnerId != networkObject.OwnerClientId){
                    DestroyFireballServerRpc();
                }
            }
            else{
                DestroyFireballServerRpc();
            }

        }
    }

    
}
