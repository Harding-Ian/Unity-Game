using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class MouseLook : NetworkBehaviour
{

    public float mouseSensitivity;

    public GameObject cameraHolder;

    float xRotation = 0f;
    float yRotation = 0f;

    Rigidbody rb;

    // Start is called before the first frame update
    
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        if(!IsLocalPlayer){
            cameraHolder.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -89.99f, 89.99f);
        
        //sets rigid body rotation
        rb.MoveRotation(Quaternion.Euler(0f, yRotation, 0f));
        //local rotation moves relative to parent --> rotation moves relative to world
        cameraHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

    }
    void ToggleCursorLock()
    {
        
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ?
                           CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = (Cursor.lockState == CursorLockMode.Locked) ? false : true;
    }
}
