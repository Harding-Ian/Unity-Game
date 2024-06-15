using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBorder : NetworkBehaviour
{
    // private void OnTriggerEnter(Collider collider){
    //     if (IsServer){
    //         if (collider.gameObject.CompareTag("Player")){
    //             WorldBoxCollidedRpc(collider.GetComponent<PlayerScript>().OwnerClientId); 
    //         }
    //     }
    // }


    private void OnTriggerEnter(Collider collider)
    {
        if (IsServer)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[collider.GetComponent<PlayerScript>().OwnerClientId].PlayerObject;
                networkObject.GetComponent<PlayerDeath>().ServerSideDeathRpc(collider.GetComponent<PlayerScript>().OwnerClientId);
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void WorldBoxCollidedRpc(ulong PlayertoDieId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[PlayertoDieId].PlayerObject;
        networkObject.GetComponent<PlayerDeath>().ServerSideDeathRpc(PlayertoDieId);
    }
}




