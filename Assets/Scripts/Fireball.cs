using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Fireball : NetworkBehaviour
{

    private ulong playerOwnerId;
    public GameObject blast;

    public GameObject gameManager;

    public Vector3 currentVelocity;
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

    void Update(){
        if (IsServer){
            currentVelocity = GetComponent<Rigidbody>().velocity;
        }

    }


    public void SetPlayerWhoFired(ulong playerId){
        playerOwnerId = playerId;
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void ApplyKnockbackRpc(Vector3 knockbackDirection, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        

        float angle = 180 - Vector3.Angle(knockbackDirection, Vector3.down) - 65;

        Debug.Log("Angle === " + angle);

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;
        Debug.Log("Adjusted angle = " + adjustedAngle);
        
        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);
        Debug.Log("Adjusted knockback direction" + adjustedknockbackDirection);
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(adjustedknockbackDirection, 50);
    }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();


    //     if (IsServer)
    //     {
    //         if (networkObject != null)
    //         {
    //             if (other.gameObject.CompareTag("projectile"))
    //             {
    //                 NetworkObject.Despawn();
    //                 GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
    //                 blastObj.GetComponent<NetworkObject>().Spawn(true);
    //             }
    //             else if (playerOwnerId != networkObject.OwnerClientId)
    //             {
    //                 if (other.gameObject.CompareTag("Player"))
    //                 {
    //                     gameManager.GetComponent<HealthManager>().applyFireballDamage(networkObject.OwnerClientId);
    //                     ApplyKnockbackRpc(GetComponent<Rigidbody>().velocity.normalized, RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp));
                        
    //                 }
    //                 NetworkObject.Despawn();
    //                 GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
    //                 blastObj.GetComponent<NetworkObject>().Spawn(true);
    //             }
    //         }
    //         else
    //         {
    //             NetworkObject.Despawn();
    //             GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
    //             blastObj.GetComponent<NetworkObject>().Spawn(true);
    //         }
            
    //     }


    // }


    private void OnCollisionEnter(Collision other){
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();

        if (IsServer)
        {
            if (networkObject != null)
            {
                if (other.gameObject.CompareTag("projectile"))
                {
                    NetworkObject.Despawn();
                    GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
                    //GameObject blastObj = Instantiate(blast, other.contacts[0].point, Quaternion.identity);
                    blastObj.GetComponent<NetworkObject>().Spawn(true);
                }
                else if (playerOwnerId != networkObject.OwnerClientId)
                {
                    if (other.gameObject.CompareTag("Player"))
                    {   
                        //ApplyKnockbackRpc(-1 * other.relativeVelocity.normalized, RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp));
                        gameManager.GetComponent<HealthManager>().applyDamage(networkObject.OwnerClientId);
                        ApplyKnockbackRpc(currentVelocity.normalized, RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp));
                        
                    }
                    NetworkObject.Despawn();
                    GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
                    //GameObject blastObj = Instantiate(blast, other.contacts[0].point, Quaternion.identity);
                    blastObj.GetComponent<NetworkObject>().Spawn(true);
                }
            }
            else
            {
                NetworkObject.Despawn();
                GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
                //GameObject blastObj = Instantiate(blast, other.contacts[0].point, Quaternion.identity);
                blastObj.GetComponent<NetworkObject>().Spawn(true);
            }
            
        }
    }

    

    
}
