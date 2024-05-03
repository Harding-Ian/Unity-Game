using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlPlayerCamera : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Vector3 offset;

    void Update()
    {
        // Only run this code if the object is controlled by the local player
        if (IsLocalPlayer)
        {
            // Move the camera holder to follow the player with an offset
            cameraHolder.transform.position = transform.position + offset;
        }
        else
        {
            // Disable the camera if it's not controlled by the local player
            cameraHolder.SetActive(false);
        }
    }
}