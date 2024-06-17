using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempChangeScene : NetworkBehaviour
{
    // Start is called before the first frame update

    private Scene scene;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Menu --> load player -> load level -> unload level/load level/u

        if (IsServer){
            if(Input.GetKeyDown(KeyCode.P)){
                NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
                var progressStatus = NetworkManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
            }
            else if(Input.GetKeyDown(KeyCode.M)){
                NetworkManager.SceneManager.UnloadScene(scene);
            }
        }
        
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log("============================================" + sceneEvent.SceneEventType);
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
        {
            Debug.Log("scene");
            scene = sceneEvent.Scene;
            NetworkManager.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;
        }           
    }
}
