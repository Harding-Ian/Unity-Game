using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CallMainMenu : NetworkBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if (NetworkManager.SceneManager == null) Debug.Log("NetworkManager.SceneManager == null");
            NetworkManager.SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }

}
