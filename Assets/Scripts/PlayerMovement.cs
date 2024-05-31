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
    public float moveSpeed;

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float dashForce;
    public float jumpCooldown;
    public float dashCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool readyToDash;
    public int jumpcount;
    public int dashcount;

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
        if(Input.GetKey(jumpKey) && readyToJump && jumpcount < 2)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
            jumpcount++;

            groundedOverride = true;
            Invoke(nameof(removeGroundedOverride), groundedOverrideTimer);
        }

        if(Input.GetKey(dashKey) && readyToDash && dashcount < 1)
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
