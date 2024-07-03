using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUIScript : NetworkBehaviour
{
    public void CallPlayerScene()
    {
        if(NetworkManager.IsHost) 
        {
            NetworkManager.SceneManager.LoadScene("PlayerScene", LoadSceneMode.Single);
        }
    }
}
