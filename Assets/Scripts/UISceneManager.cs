using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneManager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    public NetworkRelay networkRelay;
    public bool networkMenuSpawned = false;

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
        networkRelay.CreateRelayAndGameMenu(GetComponent<UISceneManager>());
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
}
