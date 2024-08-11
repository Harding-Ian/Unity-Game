using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [NonSerialized]
    public bool jump;
    [NonSerialized]
    public bool dash;

    [NonSerialized]
    public float horizontalInput;
    [NonSerialized]
    public float verticalInput;

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.Space)) jump = true;
        else jump = false;

        if(Input.GetKeyDown(KeyCode.LeftShift)) dash = true;
        else dash = false;
    }
}
