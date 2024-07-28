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
    public float constantDrag;
    public float antiMovement;

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

    private float moveSpeed;

    private bool grounded;
    private bool groundedOverride;

    float horizontalInput;
    float verticalInput;

    Vector3 inputDirection;
    Vector3 moveDirection;
    Vector3 forwardxzDir;
    Vector3 rightxzDir;
    Vector3 Velxz;

    Rigidbody rb;
    
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

        // ground check
        
        grounded = Physics.Raycast(transform.Find("GroundCheck").position, Vector3.down, groundRayLength, GroundLayer);

        

        if(groundedOverride) grounded = false;

        // handle drag
        if (grounded)
        {
            if (rb.velocity.magnitude > 0.5f){
                PlayDust = true;
            }
            else{
                PlayDust = false;
            }

            rb.drag = constantDrag;
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) rb.drag = constantDrag;
            jumpcount = 0;
            dashcount = 0;
        }
        else{
            rb.drag = constantDrag;
            PlayDust = false;
        }

        MyInput();
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

    [Rpc(SendTo.Everyone)]
    private void playDustParticlesRpc(){
        dustParticles.Play();
    }

    [Rpc(SendTo.Everyone)]
    private void stopDustParticlesRpc(){
        dustParticles.Stop();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
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

    private void MovePlayer()
    {

        Velxz = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // calculate input direction
        forwardxzDir = new Vector3(orientation.forward.x, 0f, orientation.forward.z).normalized;
        rightxzDir   = new Vector3(orientation.right.x,   0f, orientation.right.z  ).normalized;

        inputDirection = (forwardxzDir * verticalInput + rightxzDir * horizontalInput).normalized;

        //check which max speed to apply
        if(grounded) moveSpeed = stats.groundedMoveSpeed.Value;
        else moveSpeed = stats.airMoveSpeed.Value;
        moveSpeed *= stats.topspeedreduced.Value;

        //remove input component aligned with velocity if exceeding xz max speed and input is same direction as velocity
        if(Velxz.magnitude > moveSpeed && Vector3.Dot(inputDirection, Velxz) > 0)
        {
            moveDirection = inputDirection - Vector3.Project(inputDirection, Velxz);
        }
        else
        {
            moveDirection = inputDirection;
        }

        if(inputDirection.magnitude == 0f && grounded)
        {
            moveDirection = -antiMovement * Velxz.normalized;
        }
        

        // add force
        if(grounded)
        {
            rb.AddForce(moveDirection * stats.groundMoveForce.Value * stats.agilityreduced.Value, ForceMode.Acceleration);
        }
        else
            rb.AddForce(moveDirection * stats.airMoveForce.Value * stats.agilityreduced.Value, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if(rb.velocity.y < maxDownwardsJumpCancel)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - maxDownwardsJumpCancel, rb.velocity.z);
            rb.AddForce(Vector3.up * stats.jumpForce.Value, ForceMode.VelocityChange);
        }
        else if(maxDownwardsJumpCancel < rb.velocity.y && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * stats.jumpForce.Value, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(Vector3.up * stats.jumpForce.Value * (1f - stats.agilityreduced.Value)/2, ForceMode.VelocityChange);
        }
    }

    public void ApplyKnockback(Vector3 dir, float knockback)
    {
        rb.AddForce(dir.normalized * knockback * stats.knockbackBuildUp.Value, ForceMode.VelocityChange);
    }

    private void Dash()
    {

        if(rb.velocity.magnitude <= stats.airMoveSpeed.Value) rb.velocity = new Vector3(0f, 0f, 0f);
        else rb.velocity -= rb.velocity.normalized*stats.airMoveSpeed.Value;

        
        if(Vector3.Dot(orientation.forward, rb.velocity) > 0)
        {
            Vector3 forwardCorrection;
            float alignedForwardness = 0.5f * Vector3.Dot(orientation.forward.normalized, rb.velocity.normalized) + 0.5f;
            Vector3 maxForwardCorrection = orientation.forward * alignedForwardness * stats.airMoveSpeed.Value;
            Vector3 dashDirComponentOfVelocity = Vector3.Project(rb.velocity, orientation.forward);
            if(dashDirComponentOfVelocity.magnitude > maxForwardCorrection.magnitude) forwardCorrection = maxForwardCorrection;
            else forwardCorrection = dashDirComponentOfVelocity;
            rb.velocity += forwardCorrection;
        }
        rb.velocity += orientation.forward * stats.dashForce.Value;

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

}
