using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ESCMenuScript : NetworkBehaviour
{
    [NonSerialized]
    public bool inESCMenu = false;
    GameObject ESCMenuUI;
    GameObject SettingsUI;
    GameObject Crosshair;
    GameObject ReloadUI;
    GameObject ChargeUI;
    GameObject GameManager;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager = GameObject.Find("GameManager");
        ESCMenuUI = transform.Find("ESCMenuUI").gameObject;
        SettingsUI = transform.Find("SettingsUI").gameObject;
        Crosshair = transform.Find("Crosshair").gameObject;
        ReloadUI = transform.Find("ReloadUI").gameObject;
        ChargeUI = transform.Find("ChargeUI").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(SettingsUI.activeSelf)
            {
                SettingsUI.SetActive(false);
                ESCMenuUI.SetActive(true);
            }
            else
            {
                ToggleCursorLock();
            }
        }
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

    public void Quit()
    {
        if(IsServer)
        {
            GetComponent<DisconnectPlayers>().ExitLobby();
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    public void Settings()
    {
        ESCMenuUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void BackToMainUI()
    {
        ESCMenuUI.SetActive(true);
        SettingsUI.SetActive(false);
    }

}

