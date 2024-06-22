using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{

    public GameObject playerPrefab;
    public bool playersSpawned = false;


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
        if(playersSpawned == false)
        {
            int j = 0;
            foreach(ulong id in clientsCompleted)
            {
                GameObject player = Instantiate(playerPrefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
                j++;
            }
            playersSpawned = true;
        }
        teleportPlayers(clientsCompleted);
    }


    public void teleportPlayers(List<ulong> playerList)
    {
        GameObject SpawnPointHolder = GameObject.Find("SpawnPointHolder");
        List<Transform> SpawnPoints = new List<Transform>();
        for (int i = 0; i < SpawnPointHolder.transform.childCount; i++) SpawnPoints.Add(SpawnPointHolder.transform.GetChild(i));

        List<Transform> shuffledSpawnPoints = new List<Transform>();
        shuffledSpawnPoints = SpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

        int j = 0;
        foreach(ulong id in playerList)
        {
            Debug.Log(j % shuffledSpawnPoints.Count + " " + id);
            Debug.Log(shuffledSpawnPoints[j % shuffledSpawnPoints.Count]);
            Debug.Log(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position);
            
            MovePlayerRpc(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position, RpcTarget.Single(id, RpcTargetUse.Temp));
            j++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void MovePlayerRpc(Vector3 position, RpcParams rpcParams)
    {
        Debug.Log("MovePlayerRpc || moving to " + position);
        Transform playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;
        Debug.Log(playerTransform);
        Debug.Log(playerTransform.position);
        playerTransform.position = position;
        Debug.Log(playerTransform.position);
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            NetworkManager.SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }
    }


}