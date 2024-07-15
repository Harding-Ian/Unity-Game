using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Fireball : NetworkBehaviour
{

    public ulong playerOwnerId;
    private int bounces = 0;
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
        //GetComponent<Rigidbody>().detectCollisions = false;
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
    private void ApplyKnockbackRpc(Vector3 knockbackDirection, float knockbackForce, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        

        float angle = 180 - Vector3.Angle(knockbackDirection, Vector3.down) - 65;

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;
        
        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(adjustedknockbackDirection, knockbackForce);
    }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        Debug.Log("collison " + bounces);
        
        NetworkObject otherObject = collision.gameObject.GetComponent<NetworkObject>();
        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        if(otherObject == null)
        {
            if(bounces < playerWhoShot.bounces.Value)
            {
                Vector3 dir = BounceDirection();
                if(dir != Vector3.zero) GetComponent<Rigidbody>().velocity = dir * currentVelocity.magnitude * 0.5f;
                bounces++;
            }
            else
            {
                NetworkObject.Despawn();
                CreateBlast();
            }
        }
        else if(otherObject.CompareTag("projectile"))
        {
            PlayerStatsManager otherPlayerWhoShot = NetworkManager.Singleton.ConnectedClients[otherObject.GetComponent<Fireball>().playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();;
            
            if(playerWhoShot.OwnerClientId == otherPlayerWhoShot.OwnerClientId)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(),otherObject.GetComponent<Collider>());
                GetComponent<Rigidbody>().velocity = currentVelocity;
            }
            else if(playerWhoShot.orbPriority.Value > otherPlayerWhoShot.orbPriority.Value)
            {
                GetComponent<Rigidbody>().velocity = currentVelocity;
            }
            else
            {
                NetworkObject.Despawn();
                CreateBlast();
            }
        }
        else if(otherObject.CompareTag("Player") && playerOwnerId != otherObject.OwnerClientId)
        {
            gameManager.GetComponent<StatsManager>().ApplyDamage(otherObject.OwnerClientId, playerWhoShot.orbDamage.Value, playerOwnerId);
            ApplyKnockbackRpc(currentVelocity.normalized, playerWhoShot.orbKnockbackForce.Value, RpcTarget.Single(otherObject.OwnerClientId, RpcTargetUse.Temp));
            gameManager.GetComponent<StatsManager>().UpdateKnockback(otherObject.OwnerClientId, playerWhoShot.orbKnockbackPercentDamage.Value);
            PlayHitSound(playerOwnerId, otherObject.OwnerClientId);

            NetworkObject.Despawn();
            CreateBlast();
        }
        else
        {
            NetworkObject.Despawn();
            CreateBlast();
        }
    }

    private void CreateBlast()
    {
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

    private Vector3 BounceDirection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
        List<GameObject> playersInView = new List<GameObject>();
        GameObject closestPlayer = null;
        float closestdistance = 20f;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId != playerOwnerId)
            {
                var ray = new Ray(transform.position, hitCollider.gameObject.transform.position - transform.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.transform.root.CompareTag("Player") && hit.distance < closestdistance)
                {
                    closestPlayer = hitCollider.transform.root.gameObject;
                    closestdistance= hit.distance;
                }
            }
        }
        
        if (closestPlayer == null) return Vector3.zero;

        Debug.Log("closest player is " + closestPlayer.GetComponent<NetworkObject>().OwnerClientId);

        return (closestPlayer.transform.position - transform.position).normalized;
    }

    
}
