using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class NetworkedMouseLook : NetworkBehaviour
{
    public float mouseSensitivity = 500f;
    public Transform playerBody;

    private float xRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLockServerRpc();
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    [ServerRpc]
    void ToggleCursorLockServerRpc()
    {
        ToggleCursorLockClientRpc();
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ?
                           CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = (Cursor.lockState == CursorLockMode.Locked) ? false : true;
    }

    [ClientRpc]
    void ToggleCursorLockClientRpc()
    {
        // Empty client-side RPC to ensure the server's logic is executed on all clients
    }
}