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
        Debug.Log("Set id of fireball owner -------------------------------------------------------------------------------------------------------------");
        playerOwnerId = playerId;
    }

    // Update is called once per frame
    [ServerRpc]
    private void DestroyFireballServerRpc(){
        Debug.Log("destorying object");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collided)
    {
        if (IsServer)
        {
            Debug.Log("if you make contact, you better finish the job");
            NetworkObject networkObject = collided.GetComponent<NetworkObject>();
            if (networkObject != null){
                Debug.Log("Object contacted is a player.");
                Debug.Log(networkObject.OwnerClientId + "This was the id of the player hit");
                if (playerOwnerId != networkObject.OwnerClientId){
                    DestroyFireballServerRpc();
                }
            }
            else{
                DestroyFireballServerRpc();
            }
        }
        //Debug.Log("contact---------------------------------------");
    }

}
