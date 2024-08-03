using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{

    public GameObject playerPrefab;
    public bool playersSpawned = false;
    
    public ulong lastPlayerToWinId;


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

        if (sceneName == "UpgradeMap") teleportPlayers(clientsCompleted, true);
        else teleportPlayers(clientsCompleted, false);
    }


    public void teleportPlayers(List<ulong> playerList, bool UpgradeMap)
    {
        if (!IsHost) return;

        if(UpgradeMap)
        {
            playerList.Remove(lastPlayerToWinId);
            //MovePlayerRpc(SpawnPointWinner.transform.position, SpawnPointWinner.transform.rotation, UpgradeMap, RpcTarget.Single(lastPlayerToWinId, RpcTargetUse.Temp));
            Transform spawnpoint = GameObject.Find("SpawnPointWinner").transform;
            MovePlayerRpc(spawnpoint.position, spawnpoint.GetComponent<SpawnPointRotationData>().xRotation, spawnpoint.GetComponent<SpawnPointRotationData>().yRotation, UpgradeMap, RpcTarget.Single(lastPlayerToWinId, RpcTargetUse.Temp));
        }
        
        GameObject SpawnPointHolder = GameObject.Find("SpawnPointHolder");
        List<Transform> SpawnPoints = new List<Transform>();
        for (int i = 0; i < SpawnPointHolder.transform.childCount; i++) SpawnPoints.Add(SpawnPointHolder.transform.GetChild(i));

        List<Transform> shuffledSpawnPoints = new List<Transform>();
        shuffledSpawnPoints = SpawnPoints.OrderBy(x => UnityEngine.Random.value).ToList();

        int j = 0;
        foreach(ulong id in playerList)
        {
            Transform spawnpoint = shuffledSpawnPoints[j % shuffledSpawnPoints.Count];
            MovePlayerRpc(spawnpoint.position, spawnpoint.GetComponent<SpawnPointRotationData>().xRotation, spawnpoint.GetComponent<SpawnPointRotationData>().yRotation, UpgradeMap, RpcTarget.Single(id, RpcTargetUse.Temp));
            //MovePlayerRpc(shuffledSpawnPoints[j % shuffledSpawnPoints.Count].position, shuffledSpawnPoints[j % shuffledSpawnPoints.Count].rotation, UpgradeMap, RpcTarget.Single(id, RpcTargetUse.Temp));
            j++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void MovePlayerRpc(Vector3 position, float xRotation, float yRotation, bool UpgradeMap, RpcParams rpcParams)
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //player.GetComponent<ClientNetworkTransform>().Teleport(position, quaternion.identity, new Vector3(1,1,1));

        //Debug.Log("xrotation is " + xRotation);
        //Debug.Log("yrotation is " + yRotation);

        player.GetComponent<Rigidbody>().position = position;
        player.GetComponent<MouseLook>().xRotation = xRotation;
        player.GetComponent<MouseLook>().yRotation = yRotation;
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        //player.GetComponent<Rigidbody>().rotation = rotation;
        
        if(!UpgradeMap)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<Projectile>().enabled = false;
            player.GetComponent<PlayerBlock>().enabled = false;
            
            StartCoroutine(GetComponent<GameSceneManager>().StartCountdown(3));
            GetComponent<GameSceneManager>().EnableCountDownUI();
        }
    }

}
