using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainUI;//
    public GameObject SettingsUI;
    public GameObject LobbyUI;
    public void LobbyUIToMainUI()
    {
        LobbyUI.SetActive(false);
        MainUI.SetActive(true);
    }

    public void MainUIToLobbyUI()
    {
        MainUI.SetActive(false);
        LobbyUI.SetActive(true);
    }



}
