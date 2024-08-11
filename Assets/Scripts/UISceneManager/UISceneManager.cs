using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneManager : NetworkBehaviour
{

    GameObject MainMenuUI;
    GameObject MainUI;
    GameObject SettingsUI;


    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        MainMenuUI = GameObject.Find("MainMenuUI");
        MainUI = MainMenuUI.transform.Find("MainUI").gameObject;
        SettingsUI = MainMenuUI.transform.Find("SettingsUI").gameObject;


        if(!PlayerPrefs.HasKey("sensitivity")) PlayerPrefs.SetFloat("sensitivity", 2f);
        if(!PlayerPrefs.HasKey("FOV")) PlayerPrefs.SetFloat("FOV", 85f);
        if(!PlayerPrefs.HasKey("volume")) PlayerPrefs.SetFloat("volume", 0.5f);
    }


    public NetworkRelay networkRelay;
    public bool networkMenuSpawned = false;
    public bool hostButtonClickable = true;

    public void LoadNetworkMenu()
    {
        if(networkMenuSpawned == false) 
        {
            SceneManager.LoadSceneAsync("NetworkMenu", LoadSceneMode.Additive);
            networkMenuSpawned = true;
        }

    }
    public void Host()
    {
        if(hostButtonClickable) networkRelay.CreateRelayAndGameMenu(GetComponent<UISceneManager>());
        hostButtonClickable = false;
    }

    
    public void LoadGameMenu()
    {
        NetworkManager.SceneManager.LoadScene("GameMenu", LoadSceneMode.Single);
    }

    public void UnloadNetworkMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("NetworkMenu"));
        networkMenuSpawned = false;
    }

    public void Settings()
    {
        if(networkMenuSpawned) UnloadNetworkMenu();
        MainUI.SetActive(false);
        SettingsUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void BackToMainUI()
    {
        MainUI.SetActive(true);
        SettingsUI.SetActive(false);
    }


    
}
