using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class GameSceneManager : NetworkBehaviour
{
    public GameObject CountDownUI;
    public string[] maps;
    public int startCountdown;

    void Start()
    {
        if(NetworkManager.IsHost)
        {
            Debug.Log(maps);
            NetworkManager.SceneManager.LoadScene(maps[Random.Range(0, maps.Length)], LoadSceneMode.Additive);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void CountdownFinishedRpc()
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        CountDownUI.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<Projectile>().enabled = true;
        player.GetComponent<PlayerBlock>().enabled = true;
    }

    void Update()
    {
        if(!IsHost) return;

        if(Input.GetKeyDown(KeyCode.L))
        {
            CountdownFinishedRpc();
        }
    }

    public IEnumerator StartCountdown(int countdown)
    {
        
        if(countdown < 1)
        {
            CountdownFinishedRpc();
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCountdown(countdown-1);
        }
        

    }
}
