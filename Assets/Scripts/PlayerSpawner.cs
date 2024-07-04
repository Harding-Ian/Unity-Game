using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
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
        if(NetworkManager.Singleton.IsHost) NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    // public override void OnNetworkSpawn()
    // {
    //     Debug.Log("subscribed to OnLoadEventCompleted");
    //     NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    // }


    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "PlayerScene") return;
        
        if(playersSpawned == false)
        {
            foreach(ulong id in clientsCompleted)
            {
                GameObject player = Instantiate(playerPrefab);
                
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
            }
            playersSpawned = true;
        }
        teleportPlayers(clientsCompleted);
    }


    public void teleportPlayers(List<ulong> playerList)
    {
        if (!IsHost) return;
        GameObject SpawnPointHolder = GameObject.Find("SpawnPointHolder");
        List<Transform> SpawnPoints = new List<Transform>();
        for (int i = 0; i < SpawnPointHolder.transform.childCount; i++) SpawnPoints.Add(SpawnPointHolder.transform.GetChild(i));

        List<Transform> shuffledSpawnPoints = new List<Transform>();
        shuffledSpawnPoints = SpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

        int j = 0;
        foreach(ulong id in playerList)
        {
            
            //Debug.Log(j % shuffledSpawnPoints.Count + " " + id);
            //Debug.Log(shuffledSpawnPoints[j % shuffledSpawnPoints.Count]);
            //Debug.Log(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position);

            MovePlayerRpc(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position, RpcTarget.Single(id, RpcTargetUse.Temp));
            j++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void MovePlayerRpc(Vector3 position, RpcParams rpcParams)
    {
        //Debug.Log("MovePlayerRpc || moving to " + position);
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        // Debug.Log("player.IsOwner ==== " + player.IsOwner);
        // Debug.Log("player.IsOwnedByServer ==== " + player.IsOwnedByServer);
        // Debug.Log(player.transform);
        // Debug.Log(player.transform.position);
        player.transform.position = position;
        //Debug.Log(player.transform.position);

        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<Projectile>().enabled = false;
        player.GetComponent<PlayerBlock>().enabled = false;

        StartCoroutine(GetComponent<GameSceneManager>().StartCountdown(3));
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            NetworkManager.SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        }
    }


}