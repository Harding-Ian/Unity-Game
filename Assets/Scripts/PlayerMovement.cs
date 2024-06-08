using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float groundedMoveSpeed;
    public float airMoveSpeed;
    public float groundDrag;
    public float antislideDrag;
    public float airDrag;

    public float jumpForce;
    public float dashForce;
    public float jumpCooldown;
    public float dashCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool readyToDash;
    private int jumpcount;
    private int dashcount;
    public int maxjumpcount;
    public int maxdashcount;
    

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    bool groundedOverride;
    public float groundedOverrideTimer;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    public PlayerStatsManager statsManager;

    Vector3 inputDirection;
    Vector3 moveDirection;
    Vector3 forwardxzDir;
    Vector3 rightxzDir;
    Vector3 Velxz;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        readyToDash = true;
        InvokeRepeating("outputvelocity", 1f, 1f);
    }

    void outputvelocity()
    {
        if(IsLocalPlayer) Debug.Log("velocity:" + GetComponent<Rigidbody>().velocity);
    }
    

    private void Update()
    {
        if (!IsOwner) return;


        // ground check
        
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        if(groundedOverride) grounded = false;

        // handle drag
        if (grounded)
        {
            
            rb.drag = groundDrag;
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) rb.drag = antislideDrag;
            jumpcount = 0;
            dashcount = 0;
        }
        else
            rb.drag = airDrag;

        MyInput();
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
        if(Input.GetKey(jumpKey) && readyToJump && jumpcount < maxjumpcount)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            jumpcount++;

            groundedOverride = true;
            Invoke(nameof(removeGroundedOverride), groundedOverrideTimer);
        }

        if(Input.GetKey(dashKey) && readyToDash && dashcount < maxdashcount)
        {
            readyToDash = false;
            Dash();
            Invoke(nameof(ResetDash), dashCooldown);
            dashcount++;

            groundedOverride = true;
            Invoke(nameof(removeGroundedOverride), groundedOverrideTimer);
        }

    }

    private void MovePlayer()
    {

        Velxz = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // calculate input direction
        forwardxzDir = new Vector3(orientation.forward.x, 0, orientation.forward.z).normalized;
        rightxzDir   = new Vector3(orientation.right.x,   0, orientation.right.z  ).normalized;

        inputDirection = (forwardxzDir * verticalInput + rightxzDir * horizontalInput).normalized;

        //chech which max speed to apply
        if(grounded) moveSpeed = groundedMoveSpeed;//
        else moveSpeed = airMoveSpeed;

        //remove input component aligned with velocity if exceeding xz max speed and input is same direction as velocity
        if(Velxz.magnitude > moveSpeed && Vector3.Dot(inputDirection, Velxz) > 0)
            moveDirection = inputDirection - Vector3.Project(inputDirection, Velxz);
        else
            moveDirection = inputDirection;

        // add force
        if(grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        else
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void ApplyKnockback(Vector3 dir, int knockback)
    {
        rb.AddForce(dir.normalized * knockback * statsManager.knockbackBuildUp.Value, ForceMode.Impulse);
    }

    private void Dash()
    {
        // reset  velocity
        rb.velocity = new Vector3(0f, 0f, 0f);

        rb.AddForce(orientation.forward * dashForce, ForceMode.Impulse);
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
