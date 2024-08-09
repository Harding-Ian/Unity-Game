using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ESCMenuScript : MonoBehaviour
{

    [NonSerialized]
    public bool inESCMenu = false;

    GameObject ESCMenuUI;
    GameObject Crosshair;
    GameObject ReloadUI;
    GameObject ChargeUI;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameObject ScreenUI = GameObject.Find("ScreenUI");
        ESCMenuUI = ScreenUI.transform.Find("ESCMenuUI").gameObject;
        Crosshair = ScreenUI.transform.Find("Crosshair").gameObject;
        ReloadUI = ScreenUI.transform.Find("ReloadUI").gameObject;
        ChargeUI = ScreenUI.transform.Find("ChargeUI").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ToggleCursorLock();
    }


    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ESCMenuUI.SetActive(true);
            Crosshair.GetComponent<CanvasGroup>().alpha = 0f;
            ReloadUI.GetComponent<CanvasGroup>().alpha = 0f;
            ChargeUI.GetComponent<CanvasGroup>().alpha = 0f;
            inESCMenu = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ESCMenuUI.SetActive(false);
            Crosshair.GetComponent<CanvasGroup>().alpha = 1f;
            ReloadUI.GetComponent<CanvasGroup>().alpha = 1f;
            ChargeUI.GetComponent<CanvasGroup>().alpha = 1f;
            inESCMenu = false;
        }
    }

    public void Resume()
    {
        Invoke(nameof(ToggleCursorLock), 0.1f);
    }
}

