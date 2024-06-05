using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

public class Fireball : NetworkBehaviour
{

    //public NetworkVariable<ulong> playerOwnerId = new NetworkVariable<ulong>(100000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
            Invoke("DestroyProjectile", 5);
        }
    }

    public void SetPlayerWhoFired(ulong playerId){
        //playerOwnerId.Value = playerId;
        playerOwnerId = playerId;
        Debug.Log("playerownerid set to" + playerId);
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void ApplyKnockbackRpc(Vector3 knockbackDirection, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(knockbackDirection, 75);
    }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();


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
                    if (other.gameObject.CompareTag("Player"))
                    {
                        gameManager.GetComponent<HealthManager>().applyFireballDamage(networkObject.OwnerClientId);
                        Debug.Log(GetComponent<Rigidbody>().velocity.normalized);
                        ApplyKnockbackRpc(GetComponent<Rigidbody>().velocity.normalized ,RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp));
                    }
                    NetworkObject.Despawn();
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
