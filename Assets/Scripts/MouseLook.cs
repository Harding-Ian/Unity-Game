using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class MouseLook : NetworkBehaviour
{

    public float mouseSensitivity = 500f;

    public Transform orientation;

    float xRotation = 0f;
    float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        //playerBody.Rotate(Vector3.up * mouseX);
        
    }
    void ToggleCursorLock()
    {
        
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ?
                           CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = (Cursor.lockState == CursorLockMode.Locked) ? false : true;
    }
}
