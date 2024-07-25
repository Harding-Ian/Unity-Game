using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class MouseLook : NetworkBehaviour
{

    public float mouseSensitivity;

    public GameObject playerCamera;
    public GameObject cameraHolder;

    public float xRotation = 0f;
    public float yRotation = 0f;

    Rigidbody rb;

    // Start is called before the first frame update
    
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        if(!IsLocalPlayer)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
        }
        playerCamera.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;

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
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        rb.MoveRotation(Quaternion.Euler(0f, yRotation, 0f));
        //local rotation moves relative to parent --> rotation moves relative to world
        

    }


    void ToggleCursorLock()
    {
        
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ?
                           CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = (Cursor.lockState == CursorLockMode.Locked) ? false : true;
    }

    // public void ToggleSpectateOn(){
    //     if(IsLocalPlayer){
    //         cameraHolder.SetActive(false);
    //     }
    //     else{
    //         cameraHolder.SetActive(true);
    //     }
    // }
}
