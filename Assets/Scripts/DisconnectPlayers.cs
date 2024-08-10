using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DisconnectPlayers : NetworkBehaviour
{
    public void ExitLobby()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            Debug.Log("NetworkManager.Singleton.ConnectedClientsList.Count == 1");
            shutdownAndReturn();
        }
        else
        {
            Debug.Log("exitlobby");
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            disconnectClientsRpc();
        }
    }

    private void OnClientDisconnectCallback(ulong id){
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 3){
            shutdownAndReturn();
        }

    }

    private void shutdownAndReturn(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    [Rpc(SendTo.NotServer)]
    private void disconnectClientsRpc()
    {
        Debug.Log("disconnectClientsRpc");
        shutdownAndReturn();
    }

}
