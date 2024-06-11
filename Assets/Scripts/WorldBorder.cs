using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBorder : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collider){
        if (IsServer){
            if (collider.gameObject.CompareTag("Player")){
                WorldBoxCollidedRpc(collider.GetComponent<PlayerScript>().OwnerClientId); 
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void WorldBoxCollidedRpc(ulong clientId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        networkObject.GetComponent<PlayerDeath>().initiateDeathRpc(1, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        Debug.Log("Player " + clientId + " died");
    }
}
