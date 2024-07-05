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

        if (sceneName == "UpgradeMap") teleportPlayers(clientsCompleted, false);
        else teleportPlayers(clientsCompleted, true);
    }


    public void teleportPlayers(List<ulong> playerList, bool countdown)
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
            Debug.Log("Running MovePlayerRpc with " + shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position + " " + countdown + " " + id);
            MovePlayerRpc(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position, countdown, RpcTarget.Single(id, RpcTargetUse.Temp));
            j++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void MovePlayerRpc(Vector3 position, bool countdown, RpcParams rpcParams)
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        Debug.Log("Setting player.transform.position to " + position);
        player.GetComponent<Rigidbody>().position = position;
        Debug.Log("player.transform.position is " + player.transform.position);

        if(countdown)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<Projectile>().enabled = false;
            player.GetComponent<PlayerBlock>().enabled = false;
            
            StartCoroutine(GetComponent<GameSceneManager>().StartCountdown(3));
            GetComponent<GameSceneManager>().EnableCountDownUI();
        }
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("==================================================");
            List<ulong> PlayerList = new List<ulong>();
            foreach(var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
            {
                PlayerList.Add(instance.clientId.Value);
            }
            teleportPlayers(PlayerList, false);
        }
    }
}
