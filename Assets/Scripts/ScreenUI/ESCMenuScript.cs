using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ESCMenuScript : NetworkBehaviour
{
    [NonSerialized]
    public bool inESCMenu = false;
    public GameObject ESCMenuUI;
    public GameObject SettingsUI;
    public GameObject Crosshair;
    public GameObject ReloadUI;
    public GameObject ChargeUI;
    public GameObject GameManager;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (!inESCMenu)
        {


            ESCMenuUI.SetActive(true);
            Crosshair.GetComponent<CanvasGroup>().alpha = 0f;
            ReloadUI.GetComponent<CanvasGroup>().alpha = 0f;
            ChargeUI.GetComponent<CanvasGroup>().alpha = 0f;
            inESCMenu = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            
            ESCMenuUI.SetActive(false);
            Crosshair.GetComponent<CanvasGroup>().alpha = 1f;
            ReloadUI.GetComponent<CanvasGroup>().alpha = 1f;
            ChargeUI.GetComponent<CanvasGroup>().alpha = 1f;
            inESCMenu = false;

            if(IsHost && GameManager.GetComponent<EndGameManager>().inEndGameScreen) return;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

