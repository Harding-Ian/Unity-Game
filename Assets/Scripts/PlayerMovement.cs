using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerMovement : NetworkBehaviour
{

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

    public float speed = 12f;

    public float gravity = -40f;
    public float downwardSpeed = -0.05f;
    public float dashSpeed = -12f;

    public Transform groundCheck;
    public float groundRadius = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;
    public float dashTimer = 0f;

    Vector3 velocity;
    Vector3 dashModifier;
    Vector3 cameraDirection;

    bool isGrounded;
    bool isDashing;
    bool canDash;

    public override void OnNetworkSpawn() {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + " " + newValue.message);
        };
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T)) {
            randomNumber.Value = new MyCustomData {
                _int = Random.Range(1, 100),
                _bool = false,
                message = "RIP"
            };
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        dashTimer += Time.deltaTime;

        if (dashTimer > 0.3)
        {
            isDashing = false;
        }
        
        if (dashTimer > 2)
        {
            canDash = true;
        }

        
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
}
