using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System;

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

    [NonSerialized] public NetworkVariable<int> winCondition = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private string[] victoryString;

    [SerializeField]
    private List<string> victoryList2 = new List<string>();


    public bool spectatingBool = true;

   // private Scene mapScene;

    void Start()
    {
        if(NetworkManager.IsHost)
        {
            //NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            NetworkManager.SceneManager.LoadScene(maps[UnityEngine.Random.Range(0, maps.Length)], LoadSceneMode.Additive);
        }
    }

    public void InitiateGameCompletion(GameObject winner)
    {
        List<string> randomBM = new List<string>();
        randomBM = victoryList2.OrderBy(x => UnityEngine.Random.value).ToList();
        
        GetComponent<EndGameManager>().GameCompletedRpc(winner.GetComponent<PlayerScript>().clientId.Value.ToString(), randomBM[0], randomBM[1],randomBM[2]);
    }

    public void RoundCompleted(GameObject winner)
    {
        spectatingBool = false;
        if (!IsHost) return;

        GetComponent<PlayerSpawner>().lastPlayerToWinId = winner.GetComponent<PlayerScript>().clientId.Value;
        winner.GetComponent<PlayerScript>().wins.Value++;

        if(winner.GetComponent<PlayerScript>().wins.Value >= winCondition.Value) 
        {
            InitiateGameCompletion(winner);
        }
        else 
        {
            EnableRoundWinUIRpc(winner.GetComponent<PlayerScript>().clientId.Value.ToString());
            Invoke(nameof(LoadUpgradeMap), 5);
        }
    }


    [Rpc(SendTo.Everyone)]
    private void EnableRoundWinUIRpc(string winner)
    {
        roundWinnerText.text = winner + " Wins The Round";
        roundWinnerUI.SetActive(true);
    }

    private void LoadUpgradeMap()
    {
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

        StartCoroutine(LoadUpgradeScene());
        DisableRoundWinUIRpc();
        ResetPlayers();
    }

    private IEnumerator LoadUpgradeScene()
    {
        // Ensure the unload has completed
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);
        NetworkManager.SceneManager.LoadScene("UpgradeMap", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Everyone)]
    public void DisableRoundWinUIRpc()
    {
        roundWinnerUI.SetActive(false);
    }

    private void ResetPlayers()
    {
        //List<ulong> PlayerList = new List<ulong>();
        foreach(var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            instance.GetComponent<PlayerDeath>().playerSpectatingId.Value = instance.GetComponent<PlayerScript>().clientId.Value;
            instance.GetComponent<PlayerScript>().dead.Value = false;
            instance.GetComponent<PlayerStatsManager>().playerHealth.Value = instance.GetComponent<PlayerStatsManager>().maxPlayerHealth.Value;
            instance.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value = 1f;
            instance.GetComponent<PlayerScript>().upgraded.Value = false;
            if (instance.GetComponent<PlayerScript>().clientId.Value == GetComponent<PlayerSpawner>().lastPlayerToWinId) instance.GetComponent<PlayerScript>().upgraded.Value = true;
        }

        NetworkManager.LocalClient.PlayerObject.GetComponent<Projectile>().resetSlidersRpc();
        UpgradeMapPlayerStateRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpgradeMapPlayerStateRpc()
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        player.GetComponent<PlayerInput>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<Projectile>().enabled = false;
        player.GetComponent<PlayerBlock>().enabled = false;
    }


    private IEnumerator NextMap()
    {
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);
        NetworkManager.SceneManager.LoadScene(maps[UnityEngine.Random.Range(0, maps.Length)], LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Everyone)]
    public void CountdownFinishedRpc()
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        CountDownUI.SetActive(false);
        player.GetComponent<PlayerInput>().enabled = true;
        player.GetComponent<Projectile>().enabled = true;
        player.GetComponent<PlayerBlock>().enabled = true;
    }

    public void EnableCountDownUI()
    {
        CountDownUI.SetActive(true);
    }

    public IEnumerator StartCountdown(int countdown)
    {
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

    public void checkAllUpgraded()
    {
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.upgraded.Value == false) return;
        }
        spectatingBool = true;
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        StartCoroutine(NextMap());
    }

    public void ExtendWinCondition(int increment)
    {
        winCondition.Value += increment;
        LoadUpgradeMap();
        GetComponent<EndGameManager>().ExitEndGameScreenRpc();
    }

}
