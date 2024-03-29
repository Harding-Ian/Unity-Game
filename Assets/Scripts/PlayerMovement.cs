using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

<<<<<<< Updated upstream

    [SerializeField] private Transform spawnedObjectPrefab;
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 56,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public struct MyCustomData : INetworkSerializable{
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public CharacterController controller;
=======
    public float groundDrag;
>>>>>>> Stashed changes

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

<<<<<<< Updated upstream
    public override void OnNetworkSpawn() {
        // randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
        //     Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + " " + newValue.message);
        // };
    }

=======
    float horizontalInput;
    float verticalInput;
>>>>>>> Stashed changes

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
<<<<<<< Updated upstream
        if (!IsOwner) return;

        // if (Input.GetMouseButtonDown(0)) {
        //     FireballObjectServerRpc();
        // }

        if (Input.GetKeyDown(KeyCode.E)) {
            FireballObjectServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            TestServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            TestClientRpc();
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        dashTimer += Time.deltaTime;

        if (dashTimer > 0.3)
=======
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
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
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
>>>>>>> Stashed changes
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

<<<<<<< Updated upstream
        
        if (Input.GetKey(KeyCode.LeftShift) && canDash) //check for player attempting to dash
        {
            isDashing = true;
            canDash = false;
            dashTimer = 0f;
            // Transform cameraTransform = Camera.main.transform;
            // dashModifier = cameraTransform.forward * 10f;
        }

        if (isDashing) //if dashing add speed
        {
            velocity += dashModifier;
            velocity.y = -2f; // reset y vel so u dont fly into the air
        }


        if (isGrounded)
        {
            velocity.y = -2f;
        }

        if (Input.GetButton("Jump") && isGrounded)
        { 
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            velocity.y += downwardSpeed; // Apply the downward velocity
        }


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
     }

    //only runs on the server
    //must be in network behaviour class, attached to a game object with a network object
    //method name must end in ____ServerRpc
    [ServerRpc]
     private void TestServerRpc() {
        Debug.Log("test rpc: " + OwnerClientId);
     }


    //client cannot call client rpc!!
    [ClientRpc]
     private void TestClientRpc(){
        Debug.Log("test client rpc");
     }

     [ServerRpc]
     private void FireballObjectServerRpc(){
        Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
     }
}
=======
    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
>>>>>>> Stashed changes
