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

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float dashForce;
    public float jumpCooldown;
    public float dashCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool readyToDash;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

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

        //if (Input.GetMouseButtonDown(0)) FireballObjectServerRpc();
        if (Input.GetKeyDown(KeyCode.E)) FireballObjectServerRpc();
        if (Input.GetKeyDown(KeyCode.T)) TestServerRpc();
        if (Input.GetKeyDown(KeyCode.C)) TestClientRpc();

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        MyInput();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        //SpeedControl();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(dashKey) && readyToDash)
        {
            readyToDash = false;
            Dash();
            Invoke(nameof(ResetDash), dashCooldown);
        }

    }

    private void MovePlayer()
    {
        // calculate movement direction
        Vector3 moveforward = orientation.forward;
        moveforward = new Vector3(moveforward.x, 0, moveforward.z);

        Vector3 moveright = orientation.right;
        moveright = new Vector3(moveright.x, 0, moveright.z);

        moveDirection = moveforward.normalized * verticalInput + moveright.normalized * horizontalInput;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // on ground
        if(grounded && flatVel.magnitude < moveSpeed)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded && flatVel.magnitude < moveSpeed)
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
     

    //only runs on the server
    //must be in network behaviour class, attached to a game object with a network object
    //method name must end in ____ServerRpc
    [ServerRpc]
     private void TestServerRpc() {
        Debug.Log("test rpc: " + OwnerClientId);
     }


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

    public override void OnNetworkSpawn() {
        // randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
        //     Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + " " + newValue.message);
        // };
    }

}
