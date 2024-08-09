using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;

public class MouseLook : NetworkBehaviour
{

    public float mouseSensitivity;

    public GameObject playerCamera;
    public GameObject cameraHolder;

    public float xRotation = 0f;
    public float yRotation = 0f;

    Rigidbody rb;
    GameObject ScreenUI;


    
    void Start()
    {
        if(!IsLocalPlayer) playerCamera.GetComponent<Camera>().enabled = false;
        if(!IsLocalPlayer) return;
        ScreenUI = GameObject.Find("ScreenUI");
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        if(!IsLocalPlayer) return;

        if(InESCMenu()) return;

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

    public bool InESCMenu()
    {
        return ScreenUI.GetComponent<ESCMenuScript>().inESCMenu;
    }

}
