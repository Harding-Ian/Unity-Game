using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Drag")]
    public float slowMultiplierHeight;
    public float slowMultiplierWidth;
    public float slowDrag;
    public float fastDrag;
    public float idleAirDrag;
    public float yUpDrag;
    public float yDownDrag;

    [Header("Jump")]
    public float jumpCooldown;
    public float maxDownwardsJumpCancel; //decide later

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float groundRayLength;
    public LayerMask GroundLayer;
    public float groundedOverrideTimer;

    [Header("References")]
    public Transform orientation;
    public PlayerStatsManager stats;
    public ParticleSystem dustParticles;



    private bool readyToJump;
    private bool readyToDash;
    private int jumpcount;
    private int dashcount;

    private bool grounded;
    private bool groundedOverride;

    float horizontalInput;
    float verticalInput;

    Vector3 inputDirection;
    Vector3 moveDirection;
    Vector3 forwardxzDir;
    Vector3 rightxzDir;
    Vector3 Velxz;
    Vector3 Vely;

    Rigidbody rb;

    private int stunHitCounter = 0;
    private int stunInvokeCounter = 1;
    
    private bool playDust  = false;



    private bool PlayDust
    {
        get { return playDust; }
        set 
        {
            if(playDust == value) return;
            playDust = value;

            if (value) playDustParticlesRpc();
            else stopDustParticlesRpc();
        }
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        readyToDash = true;
    }
    

    private void Update()
    {
        if (!IsLocalPlayer) return;

        grounded = Physics.Raycast(transform.Find("GroundCheck").position, Vector3.down, groundRayLength, GroundLayer);
        if(groundedOverride) grounded = false;

        if (grounded)
        {
            if (rb.velocity.magnitude > 0.5f) PlayDust = true;
            else PlayDust = false;


            jumpcount = 0;
            dashcount = 0;
        }
        else
        {
            PlayDust = false;
        }

        MyInput();
    }

    private void FixedUpdate()
    {
        VelComponents();
        MovePlayer();
        ApplyDrag();
    }


    [Rpc(SendTo.Everyone)]
    private void playDustParticlesRpc(){
        dustParticles.Play();
    }

    [Rpc(SendTo.Everyone)]
    private void stopDustParticlesRpc(){
        dustParticles.Stop();
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        if(Input.GetKeyDown(jumpKey) && readyToJump && jumpcount < stats.numberOfJumps.Value)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            jumpcount++;

            groundedOverride = true;
            Invoke(nameof(removeGroundedOverride), groundedOverrideTimer);
        }


        if(Input.GetKeyDown(dashKey) && readyToDash && dashcount < stats.numberOfDashes.Value)
        {
            readyToDash = false;
            Dash();
            Invoke(nameof(ResetDash), stats.dashCooldown.Value);
            dashcount++;

            groundedOverride = true;
            Invoke(nameof(removeGroundedOverride), groundedOverrideTimer);
        }
    }

    private void VelComponents()
    {
        Velxz = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vely = new Vector3(0f, rb.velocity.y, 0f);
    }

    private void MovePlayer()
    {
        float moveSpeed;
        float moveForce;
        // calculate input direction
        forwardxzDir = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;
        rightxzDir   = new Vector3(orientation.right.x,   0f, orientation.right.z  ).normalized;

        inputDirection = (forwardxzDir * verticalInput + rightxzDir * horizontalInput).normalized;


        //check which max speed to apply
        if(grounded) 
        {
            moveSpeed = stats.groundedMoveSpeed.Value;
            moveForce = stats.groundMoveForce.Value;
        }
        else 
        {
            moveSpeed = stats.airMoveSpeed.Value;
            moveForce = stats.airMoveForce.Value;
        }
        moveSpeed *= stats.topSpeedMultiplier.Value;


        //remove input component aligned with velocity if exceeding xz max speed and input is same direction as velocity
        if(Velxz.magnitude > moveSpeed && Vector3.Dot(inputDirection, Velxz) > 0f)
        {
            moveDirection = inputDirection - Vector3.Project(inputDirection, Velxz);
        }
        else
        {
            moveDirection = inputDirection;
        }


        float a = moveSpeed * (slowMultiplierHeight/9f);
        float b = slowMultiplierWidth;
        float slowmultiplier = (float)(a * (Math.Exp(-(b/a)*(b/a)*Velxz.magnitude*Velxz.magnitude) + 1f/a));
        

        rb.AddForce(slowmultiplier * moveDirection * moveForce * stats.agilityMultiplier.Value, ForceMode.Acceleration);
    }


    private void ApplyDrag()
    {
        float xzForce;
        float yForce;
        float topspeed;

        if(grounded) topspeed = stats.groundedMoveSpeed.Value;
        else topspeed = stats.airMoveSpeed.Value;

        //xzVel
        if(!grounded && verticalInput == 0f && horizontalInput == 0f && Velxz.magnitude < topspeed) xzForce = idleAirDrag;
        else if(Velxz.magnitude < 0.2f && grounded) xzForce = slowDrag/(0.2f*0.2f) * Velxz.magnitude*Velxz.magnitude;
        else if(Velxz.magnitude < 0.2f && !grounded) xzForce = fastDrag/(0.2f*0.2f) * Velxz.magnitude*Velxz.magnitude;
        else if(Velxz.magnitude < topspeed && grounded) xzForce = slowDrag;
        else xzForce = (1f/4.5f)*(Velxz.magnitude - topspeed) + fastDrag;

        //yVel
        if(rb.velocity.y > 0) yForce = yUpDrag * Vely.magnitude;
        else yForce = yDownDrag * Vely.magnitude;
        

        rb.AddForce(-Velxz.normalized * xzForce + -Vely.normalized * yForce, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if(rb.velocity.y < maxDownwardsJumpCancel)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - maxDownwardsJumpCancel, rb.velocity.z);
            rb.AddForce(Vector3.up * stats.jumpForce.Value * (1f + stats.agilityMultiplier.Value)/2f, ForceMode.VelocityChange);
        }
        else if(maxDownwardsJumpCancel < rb.velocity.y && rb.velocity.y < 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * stats.jumpForce.Value * (1f + stats.agilityMultiplier.Value)/2f, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(Vector3.up * stats.jumpForce.Value * (1f + stats.agilityMultiplier.Value)/2f, ForceMode.VelocityChange);
        }
    }

    private void Dash()
    {

        if(rb.velocity.magnitude <= stats.airMoveSpeed.Value) rb.velocity = new Vector3(0f, 0f, 0f);
        else 
        {
            //rb.velocity -= rb.velocity.normalized*stats.airMoveSpeed.Value;
            rb.AddForce(-1 * rb.velocity.normalized*stats.airMoveSpeed.Value * (1f + stats.agilityMultiplier.Value)/2, ForceMode.VelocityChange);
        }

        
        if(Vector3.Dot(orientation.forward, rb.velocity) > 0)
        {
            Vector3 forwardCorrection;
            float alignedForwardness = 0.5f * Vector3.Dot(orientation.forward.normalized, rb.velocity.normalized) + 0.5f;
            Vector3 maxForwardCorrection = orientation.forward * alignedForwardness * stats.airMoveSpeed.Value;
            Vector3 dashDirComponentOfVelocity = Vector3.Project(rb.velocity, orientation.forward);
            if(dashDirComponentOfVelocity.magnitude > maxForwardCorrection.magnitude) forwardCorrection = maxForwardCorrection;
            else forwardCorrection = dashDirComponentOfVelocity;
            //rb.velocity += forwardCorrection;
            rb.AddForce(forwardCorrection * (1f + stats.agilityMultiplier.Value)/2, ForceMode.VelocityChange);
        }
        //rb.velocity += orientation.forward * stats.dashForce.Value;

        rb.AddForce(orientation.forward * stats.dashForce.Value * (1f + stats.agilityMultiplier.Value)/2, ForceMode.VelocityChange);

    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void ResetDash()
    {
        readyToDash = true;
    }

    private void removeGroundedOverride()
    {
        groundedOverride = false;
    }

    public void ReduceSpeed(float topspeedreducedInput, float agilityreducedInput, float StunTimer)
    {
        stunHitCounter++;
        if(stats.topSpeedMultiplier.Value > topspeedreducedInput) stats.topSpeedMultiplier.Value = topspeedreducedInput;
        if(stats.agilityMultiplier.Value > agilityreducedInput) stats.agilityMultiplier.Value = agilityreducedInput;
        Invoke(nameof(CheckToResetSlowValues), StunTimer);
    }

    private void ResetSlowValues()
    {
        stats.topSpeedMultiplier.Value = 1f;
        stats.agilityMultiplier.Value = 1f;

        stunHitCounter = 0;
        stunInvokeCounter = 1;
    }

    private void CheckToResetSlowValues()
    {
        if(stunHitCounter == stunInvokeCounter) ResetSlowValues();
        else stunInvokeCounter++;
    }

}
