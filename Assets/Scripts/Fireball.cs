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

    private int clusterBombs;
    public GameObject blast;

    public GameObject gameManager;

    public Vector3 currentVelocity;
    Vector3 gravity;
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

    public void setStats(){
        
    }


    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        
        NetworkObject otherObject = collision.gameObject.GetComponent<NetworkObject>();
        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        //Debug.Log("fireball " + NetworkObjectId + " collided with " + collision.gameObject + " bounce: " + bounces);

        if(otherObject == null)
        {
            if(bounces < playerWhoShot.bounces.Value)
            {
                //vel towards player
                Vector3 dir = GetComponent<CalculateBounce>().BounceDirection();
                if(dir != Vector3.zero) 
                {
                    gravity = GetComponent<ConstantForce>().force;
                    GetComponent<Rigidbody>().velocity = dir * currentVelocity.magnitude * 0.5f;
                    GetComponent<ConstantForce>().force = new Vector3(0, 0, 0);
                    Invoke(nameof(SetGravity), 0.5f);
                }

                //homing
                //GetComponent<Homing>().nearestPlayer = NearestPlayer();
                //GetComponent<Homing>().homingStrength = 50f;
                
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
            
            otherObject.GetComponent<PlayerKnockback>().ApplyKnockbackRpc(currentVelocity.normalized, playerWhoShot.orbKnockbackForce.Value, RpcTarget.Single(otherObject.OwnerClientId, RpcTargetUse.Temp));

            gameManager.GetComponent<StatsManager>().UpdateKnockback(otherObject.OwnerClientId, playerWhoShot.orbKnockbackPercentDamage.Value);
            GetComponent<FireballAudio>().PlayHitSound(playerOwnerId, otherObject.OwnerClientId);

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

        GetComponent<FireballAudio>().PlayBlastSound();
    }

    


    void SetGravity()
    {
        GetComponent<ConstantForce>().force = gravity;
    }

    
}
