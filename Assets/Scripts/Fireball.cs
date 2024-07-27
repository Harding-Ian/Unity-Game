using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Fireball : NetworkBehaviour
{

    [NonSerialized] public ulong playerOwnerId;
    private int bounces = 0;

    public GameObject blast;

    private GameObject gameManager;

    private Vector3 currentVelocity;
    Vector3 gravity;

    // ------------------------------- Orb Related Stuff -------------------------------

    [NonSerialized] public float orbDamage = 2f;

    [NonSerialized] public float orbKnockbackForce = 50f;

    [NonSerialized] public float orbKnockbackPercentDamage = 0.2f;

    [NonSerialized] public int orbPriority = 1;

    // ------------------------------- Explosion Related Stuff -------------------------------
    [NonSerialized] public float explosionDamage = 1f;

    [NonSerialized] public float explosionKnockbackForce = 12f;

    [NonSerialized] public float explosionKnockbackPercentDamage = 0.1f;

    [NonSerialized] public float explosionRadius = 4f;

    

    // ------------------------------- Altered Mechanics Related Stuff -------------------------------

    [NonSerialized] public float homing = 0f;

    [NonSerialized] public int maxBounces = 0;

    [NonSerialized] public int clusterBomb = 0;

    public void SetDamageStats(float orbDamagePlayer, float orbKnockbackForcePlayer, float orbKnockbackPercentDamagePlayer, int orbPriorityPlayer)
    {
        orbDamage = orbDamagePlayer;
        orbKnockbackForce = orbKnockbackForcePlayer;
        orbKnockbackPercentDamage = orbKnockbackPercentDamagePlayer;
        orbPriority = orbPriorityPlayer;
    }

    public void SetExplosionStats(float explosionDamagePlayer, float explosionKnockbackForcePlayer, float explosionKnockbackPercentDamagePlayer, float explosionRadiusPlayer)
    {
        explosionDamage = explosionDamagePlayer;
        explosionKnockbackForce = explosionKnockbackForcePlayer;
        explosionKnockbackPercentDamage = explosionKnockbackPercentDamagePlayer;
        explosionRadius = explosionRadiusPlayer;
    }

    public void SetSpecialStats(float homingPlayer, int maxBouncesPlayer, int clusterBombPlayer)
    {
        homing = homingPlayer;
        maxBounces = maxBouncesPlayer;
        clusterBomb = clusterBombPlayer;
    }

    public void SetPlayerOwnerId(ulong playerId)
    {
        playerOwnerId = playerId;
    }
    

    void Start()
    {   
        if (!IsServer) return;
        
        gameManager = GameObject.Find("GameManager");
        currentVelocity = GetComponent<Rigidbody>().velocity;
        Invoke(nameof(DespawnProjectile), 5);
    }

    void Update()
    {
        if (!IsServer) return;
        currentVelocity = GetComponent<Rigidbody>().velocity;
    }


    private void DespawnProjectile()
    {
        NetworkObject.Despawn();
    }

    private void DestroyProjectile(Vector3 normal)
    {
        if (clusterBomb > 0) GetComponent<ClusterBomb>().spawnClusterBombs(normal);
        NetworkObject.Despawn();
        CreateBlast();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        //Debug.Log("fireball " + NetworkObjectId + " collided with " + collision.gameObject + " bounce: " + bounces);
        
        NetworkObject otherObject = collision.gameObject.GetComponent<NetworkObject>();
        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        ContactPoint contact = collision.contacts[0];
        Vector3 normal = contact.normal;

        if(otherObject == null)
        {
            if(bounces < maxBounces)
            {
                Vector3 dir = GetComponent<CalculateBounce>().BounceDirection();
                if(dir != Vector3.zero) 
                {
                    gravity = GetComponent<ConstantForce>().force;
                    GetComponent<Rigidbody>().velocity = dir * currentVelocity.magnitude * 0.5f;
                    GetComponent<ConstantForce>().force = new Vector3(0, 0, 0);
                    Invoke(nameof(SetGravity), 0.5f);
                }
                bounces++;
            }
            else
            {
                DestroyProjectile(normal);
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
            else if(orbPriority > otherObject.GetComponent<Fireball>().orbPriority)
            {
                GetComponent<Rigidbody>().velocity = currentVelocity;
            }
            else
            {
                DestroyProjectile(normal);
            }
        }
        else if(otherObject.CompareTag("Player") && playerOwnerId != otherObject.OwnerClientId)
        {
            gameManager.GetComponent<StatsManager>().ApplyDamage(otherObject.OwnerClientId, orbDamage, playerOwnerId);
            
            otherObject.GetComponent<PlayerKnockback>().ApplyKnockbackRpc(currentVelocity.normalized, orbKnockbackForce, false, RpcTarget.Single(otherObject.OwnerClientId, RpcTargetUse.Temp));

            gameManager.GetComponent<StatsManager>().UpdateKnockback(otherObject.OwnerClientId, orbKnockbackPercentDamage);
            GetComponent<FireballAudio>().PlayHitSound(playerOwnerId, otherObject.OwnerClientId);
            
            DestroyProjectile(normal);
        }
        else
        {
            DestroyProjectile(normal);
        }
    }

    private void CreateBlast()
    {
        GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
        blastObj.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
        blastObj.GetComponent<ProjectileBlast>().SetStats(explosionRadius, explosionDamage, explosionKnockbackPercentDamage, explosionKnockbackForce);
        blastObj.GetComponent<NetworkObject>().Spawn(true);
        blastObj.GetComponent<ProjectileBlast>().SetPlayerWhoFired(playerOwnerId);

        GetComponent<FireballAudio>().PlayBlastSound();
    }

    


    void SetGravity()
    {
        GetComponent<ConstantForce>().force = gravity;
    }

    
}
