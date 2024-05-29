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
        NetworkObject.Despawn();
        Destroy(gameObject);
    }

    // [ServerRpc]
    // private void DestroyOtherProjectileServerRpc(ulong networkObjectId){
    //     NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    //     if (networkObject != null)
    //     {
    //         networkObject.Despawn();
    //         Destroy(networkObject.gameObject);
    //     }
    // }

    private void OnCollisionEnter(Collision collision){
        if (IsServer)
        {   
            NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();
            if (networkObject != null){
                if(collision.gameObject.CompareTag("projectile")){
                    Debug.Log("Projectiles Collided");
                    DestroyFireballServerRpc();
                    //DestroyOtherProjectileServerRpc(networkObject.NetworkObjectId);
                }
                else if (playerOwnerId != networkObject.OwnerClientId){
                    DestroyFireballServerRpc();
                    Debug.Log("object is networked");
                    if(collision.gameObject.CompareTag("Player")){
                        Debug.Log("PLAYER HIT!!!!!!");
                    }
                }
            }
            else{
                DestroyFireballServerRpc();
                Debug.Log("object is not networked");
            }

        }
    }

    

    
}
