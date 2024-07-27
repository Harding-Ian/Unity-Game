using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class DecoyScript : NetworkBehaviour
{
    public GameObject blast;
    Vector3 moveDirection;
    Rigidbody rb;
    private float forwardForce = 0;
    private float maxSpeed = 0;

    public ParticleSystem dustParticles;

    private bool grounded;
    public float groundRayLength;
    public LayerMask GroundLayer;
    public float groundDrag;
    public float airDrag;
    public quaternion rotation;
    private ulong playerOwnerId;


    // ------------------------------- Explosion Related Stuff -------------------------------
    [NonSerialized] public float explosionDamage = 1f;

    [NonSerialized] public float explosionKnockbackForce = 12f;

    [NonSerialized] public float explosionKnockbackPercentDamage = 0.1f;

    [NonSerialized] public float explosionRadius = 4f;



    public GameObject audioSrcPrefab;


    private bool playDust  = false;

    private bool PlayDust
    {
        get { return playDust;}
        set {
            if(playDust != value)
            {
                playDust = value;
                HandleDustUpdate(value);
            }
        }
    }

    private void HandleDustUpdate(bool condition)
    {
        if (condition)
        {
            playDustParticlesRpc();
        }
        else
        {
            stopDustParticlesRpc();
        }
    }
    
    public void SetExplosionStats(float explosionRadiusInput, float explosionDamageInput, float explosionKnockbackPercentDamageInput, float explosionKnockbackForceInput)
    {
        explosionRadius = explosionRadiusInput;
        explosionDamage = explosionDamageInput;
        explosionKnockbackPercentDamage = explosionKnockbackPercentDamageInput;
        explosionKnockbackForce = explosionKnockbackForceInput;
    }

    public void SetMovementStats(float inputForce, float inputSpeed, Vector3 direction, Quaternion inputRotation)
    {
        forwardForce = inputForce;
        maxSpeed = inputSpeed;
        moveDirection = direction;
        rotation = inputRotation;
    }

    public void SetPlayerOwnerId(ulong playerOwnerIdInput)
    {
        playerOwnerId = playerOwnerIdInput;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(destroyDecoy), 5);
        rb.drag = groundDrag;
    }


    private void destroyDecoy()
    {
        NetworkObject.Despawn();
        CreateBlast();
    }


    private void CreateBlast()
    {
        GameObject blastObj = Instantiate(blast, GetComponent<Transform>().position, Quaternion.identity);
        blastObj.transform.localScale = new Vector3(2 * explosionRadius, 2 * explosionRadius, 2 * explosionRadius);
        blastObj.GetComponent<ProjectileBlast>().SetStats(explosionRadius, explosionDamage, explosionKnockbackPercentDamage, explosionKnockbackForce);
        blastObj.GetComponent<NetworkObject>().Spawn(true);
        blastObj.GetComponent<ProjectileBlast>().SetPlayerWhoFired(playerOwnerId);

        PlayBlastSound();
    }

    public void PlayBlastSound()
    {
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



    void Update()
    {
        if(!IsServer) return;

        grounded = Physics.Raycast(transform.Find("GroundCheck").position, Vector3.down, groundRayLength, GroundLayer);
        


        // handle drag
        if(grounded)
        {
            PlayDust = true;
            rb.drag = groundDrag;
        }
        else
        {
            PlayDust = false;
            rb.drag = airDrag;
        }

        if(rb.velocity.magnitude < maxSpeed) rb.AddForce(moveDirection * forwardForce, ForceMode.Acceleration);
        rb.MoveRotation(Quaternion.LookRotation(moveDirection));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;

        if (collision.gameObject.CompareTag("projectile"))
        {
            GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
            audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);

            SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
            soundPlayer.PlayDecoySound(collision.gameObject.GetComponent<NetworkObject>().OwnerClientId);
            destroyDecoy();
        }
    }
    

    [Rpc(SendTo.Everyone)]
    private void playDustParticlesRpc(){
        dustParticles.Play();
    }

    [Rpc(SendTo.Everyone)]
    private void stopDustParticlesRpc(){
        dustParticles.Stop();
    }
}
