using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DecoyScript : NetworkBehaviour
{
    Vector3 moveDirection;
    Rigidbody rb;
    private float forwardForce = 0;
    private float maxSpeed = 0;

    public ParticleSystem dustParticles;

    private bool grounded;
    public float playerHeight;
    public LayerMask whatIsGround;
    public float groundDrag;
    public float airDrag;
    public quaternion rotation;


    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(destroyDecoy), 5);
        rb.drag = groundDrag;
    }

    private void destroyDecoy()
    {
        NetworkObject.Despawn();
    }

    public void setStats(float inputForce, float inputSpeed, Vector3 direction, Quaternion inputRotation)
    {
        forwardForce = inputForce;
        maxSpeed = inputSpeed;
        moveDirection = direction;
        rotation = inputRotation;
    }

    void Update()
    {
        if(!IsServer) return;
        playDustParticlesRpc();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);
        


        // handle drag
        if(grounded)
        {
            playDustParticlesRpc();
            rb.drag = groundDrag;
        }
        else
        {
            stopDustParticlesRpc();
            rb.drag = airDrag;
        }



        if(rb.velocity.magnitude < maxSpeed) rb.AddForce(moveDirection * forwardForce, ForceMode.Acceleration);

        rb.MoveRotation(Quaternion.LookRotation(moveDirection));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!IsServer) return;
        Debug.Log("decoy collided with " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("projectile"))
        {
            playDecoySoundRpc(RpcTarget.Single(collision.gameObject.GetComponent<Fireball>().playerOwnerId, RpcTargetUse.Temp));
            destroyDecoy();
        }
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    private void playDecoySoundRpc(RpcParams rpcParams)
    {
        Debug.Log("played decoy sound on this pc");
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
