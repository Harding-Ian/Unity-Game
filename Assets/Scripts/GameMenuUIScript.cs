using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUIScript : NetworkBehaviour
{

    // private ulong oldId;

    // void Start(){
    //     NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisconnect;
    //     oldId = OwnerClientId;
    // }

    // private void OnHostDisconnect(ulong id){
    //     Debug.Log("Thirst");
    //     Debug.Log("Id = " + id);
    //     Debug.Log("ownerclientid = " + OwnerClientId);
    //     Debug.Log("NetworkManager.ServerClientId = " + NetworkManager.ServerClientId);
    //     Debug.Log("oldId = " + oldId);
    //     if (id == oldId){
    //         Debug.Log("Hunger");
    //         shutdownAndReturn();
    //     }
    // }
    public void CallPlayerScene()
    {
        if(NetworkManager.IsHost) 
        {
            NetworkManager.SceneManager.LoadScene("PlayerScene", LoadSceneMode.Single);
        }
    }

    public void ExitLobby(){
        if (NetworkManager.Singleton.IsHost)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 1){
                shutdownAndReturn();
            }
            else{
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                disconnectClientsRpc();
            }
        }
        else{
            shutdownAndReturn();
        }
    }

    private void OnClientDisconnectCallback(ulong id){
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 3){
            shutdownAndReturn();
        }

    }

    private void shutdownAndReturn(){
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    [Rpc(SendTo.NotServer)]
    private void disconnectClientsRpc(){
        shutdownAndReturn();
    }

    public void ContinueGame(){
        if (NetworkManager.IsHost){
            int increment = 2;
            if (NetworkManager.Singleton.ConnectedClientsList.Count < 3){
                increment = 4;
            }
            else if (NetworkManager.Singleton.ConnectedClientsList.Count < 5){
                increment = 3;
            }
            GameObject.Find("GameManager").GetComponent<GameSceneManager>().ExtendWinCondition(increment);
        }
    }

}
