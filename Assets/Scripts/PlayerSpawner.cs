using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{

    public GameObject playerPrefab;


    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if(NetworkManager.IsHost) NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    // public override void OnNetworkSpawn()
    // {
    //     Debug.Log("subscribed to OnLoadEventCompleted");
    //     NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    // }


    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("SceneLoaded running");
        Debug.Log("ishost === " + IsServer);
        if(sceneName == "Main")
        {
            foreach(ulong id in clientsCompleted)
            {
                Debug.Log("id === " + id);
                GameObject player = Instantiate(playerPrefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
            }
        }
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            NetworkManager.SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }
    }


}