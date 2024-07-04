using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class GameSceneManager : NetworkBehaviour
{

    [SerializeField]
    private GameObject CountDownUI;

    [SerializeField]
    private TextMeshProUGUI countdownTMPGUI;

    [SerializeField]
    private TextMeshProUGUI roundWinnerText;

    [SerializeField]
    private GameObject roundWinnerUI;

    [SerializeField]
    private string[] maps;

   // private Scene mapScene;

    void Start()
    {
        if(NetworkManager.IsHost)
        {
           // NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            NetworkManager.SceneManager.LoadScene(maps[Random.Range(0, maps.Length)], LoadSceneMode.Additive);
        }
    }

    // private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    // {
    //     Debug.Log("Scene Event Triggered: " + sceneEvent.SceneEventType + " for scene: " + sceneEvent.Scene.name);
    //     if (sceneEvent.Scene.name == "PlayerScene" || sceneEvent.Scene.name == "") return;

    //     Debug.Log("============================================" + sceneEvent.SceneEventType);
    //     if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
    //     {
    //         Debug.Log("da da da dadada dad ada");
    //         if (sceneEvent.Scene != null){
    //             mapScene = sceneEvent.Scene;
    //         }
    //         Debug.Log(mapScene.name);
    //         //NetworkManager.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;
    //     }
    // }

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

    public void debugninja()
    {
        Debug.Log("ninja");
    }

    public void RoundCompleted(string winner)
    {
        if (IsHost)
        {
            RoundCompletedRpc(winner);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void RoundCompletedRpc(string winner)
    {
        roundWinnerText.text = winner + " Wins The Round";
        roundWinnerUI.SetActive(true);
        if (IsHost)
        {
            Invoke(nameof(EndRound), 5);
        }
    }

    private void EndRound()
    {
        if (IsHost)
        {
            //check for game winner here if enough points
        //if not go to upgrades
            NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            NetworkManager.SceneManager.LoadScene("UpgradeMap", LoadSceneMode.Additive);

            StartCoroutine(LoadUpgradeScene());
            // Debug.Log(SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name);
            EndRoundRpc();
        }
    }

    private IEnumerator LoadUpgradeScene()
    {
        // Ensure the unload has completed
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);

        Debug.Log("Loading UpgradeMap scene.");
        NetworkManager.SceneManager.LoadScene("UpgradeMap", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Everyone)]
    private void EndRoundRpc()
    {
        roundWinnerUI.SetActive(false);
    }

    public IEnumerator StartCountdown(int countdown)
    {
        Debug.Log("countdown === " + countdown);
        countdownTMPGUI.text = countdown.ToString();
        if(countdown < 1)
        {
            CountdownFinishedRpc();
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(StartCountdown(countdown-1));
        }
    }
}
