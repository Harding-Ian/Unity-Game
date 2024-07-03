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

    public GameObject audioSrcPrefab;
    void Start()
    {   
        gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found within scene");
        }
        if (IsServer){
            Invoke(nameof(DestroyProjectile), 5);
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

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;
        
        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(adjustedknockbackDirection, 50);
    }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }



    private void OnCollisionEnter(Collision other){
        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();

        if (IsServer)
        {
            if (networkObject != null)
            {
                if (other.gameObject.CompareTag("projectile"))
                {
                    NetworkObject.Despawn();
                    CreateBlast();
                }
                else if (playerOwnerId != networkObject.OwnerClientId)
                {
                    if (other.gameObject.CompareTag("Player"))
                    {   
                        gameManager.GetComponent<StatsManager>().ApplyDamage(networkObject.OwnerClientId, 2, playerOwnerId);
                        ApplyKnockbackRpc(currentVelocity.normalized, RpcTarget.Single(networkObject.OwnerClientId, RpcTargetUse.Temp));
                        gameManager.GetComponent<StatsManager>().UpdateKnockback(networkObject.OwnerClientId, 0.25f);
                        PlayHitSound(playerOwnerId, networkObject.OwnerClientId);
                    }

                    NetworkObject.Despawn();
                    CreateBlast();
                }
            }
            else
            {
                NetworkObject.Despawn();
                CreateBlast();
            }
        }
    }


    private void CreateBlast(){
        GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
        blastObj.GetComponent<NetworkObject>().Spawn(true);
        blastObj.GetComponent<ProjectileBlast>().SetPlayerWhoFired(playerOwnerId);

        PlayBlastSound();
    }

    private void PlayBlastSound(){
        GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
        audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);

        SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.PlayBlastSound();
        }
        else
        {
            Debug.LogError("SoundEffectPlayer component not found on audioSrcInstance.");
        }
    }

    private void PlayHitSound(ulong id1, ulong id2){
        GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
        audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);

        SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.OnDirectHit(id1, id2);
        }
        else
        {
            Debug.LogError("SoundEffectPlayer component not found on audioSrcInstance.");
        }
    }



    
}
