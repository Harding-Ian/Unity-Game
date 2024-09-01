using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainUI;
    public GameObject SettingsUI;
    public GameObject LobbyUI;
    public GameObject LobbyGameMenu;
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

    public void MainUIToLobbyGameMenu()
    {
        MainUI.SetActive(false);
        LobbyGameMenu.SetActive(true);
    }

    public void LobbyGameMenuToMainUI()
    {
        LobbyGameMenu.SetActive(false);
        MainUI.SetActive(true);
    }

    public void LobbyUIToLobbyGameMenu()
    {
        LobbyUI.SetActive(false);
        LobbyGameMenu.SetActive(true);
    }
}
