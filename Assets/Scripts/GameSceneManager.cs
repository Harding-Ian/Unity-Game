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

    [SerializeField]
    private int winCondition = 5;

    [SerializeField]
    private string[] victoryString;

    [SerializeField]
    private List<string> victoryList2 = new List<string>();

    [SerializeField]
    private GameObject victoryUI;

    [SerializeField]
    private TextMeshProUGUI victoryText;
    
    [SerializeField]
    private TextMeshProUGUI victoryName;

   // private Scene mapScene;

    void Start()
    {
        if(NetworkManager.IsHost)
        {
            //NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            NetworkManager.SceneManager.LoadScene(maps[Random.Range(0, maps.Length)], LoadSceneMode.Additive);
        }
    }

    private IEnumerator NextMap()
    {
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);
        NetworkManager.SceneManager.LoadScene(maps[Random.Range(0, maps.Length)], LoadSceneMode.Additive);
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

    public void EnableCountDownUI()
    {
        CountDownUI.SetActive(true);
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
            if (instance.GetComponent<PlayerScript>().clientId.Value == GetComponent<PlayerSpawner>().lastPlayerToWinId){
                instance.GetComponent<PlayerScript>().upgraded.Value = true;
            }
            else{
                instance.GetComponent<PlayerScript>().upgraded.Value = false;
            }
            //PlayerList.Add(instance.clientId.Value);
        }
        //GetComponent<PlayerSpawner>().teleportPlayers(PlayerList);
        UpgradeMapPlayerStateRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpgradeMapPlayerStateRpc()
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<Projectile>().enabled = false;
        player.GetComponent<PlayerBlock>().enabled = false;
        
        
    }

    public void RoundCompleted(GameObject winner)
    {
        if (IsHost)
        {
            GetComponent<PlayerSpawner>().lastPlayerToWinId = winner.GetComponent<PlayerScript>().clientId.Value;
            winner.GetComponent<PlayerScript>().wins.Value++;
            if(winner.GetComponent<PlayerScript>().wins.Value >= winCondition)
            {

                List<string> randomBM = new List<string>();
                randomBM = victoryList2.OrderBy(x => UnityEngine.Random.value).ToList();
                
                GameCompletedRpc(winner.GetComponent<PlayerScript>().clientId.Value.ToString(), randomBM[0], randomBM[1],randomBM[2]);
            }
            else
            {
                RoundCompletedRpc(winner.GetComponent<PlayerScript>().clientId.Value.ToString());
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void GameCompletedRpc(string winner, string bm1, string bm2, string bm3)
    {
        victoryText.text = bm1;
        victoryName.text = winner + " Wins";
        victoryUI.SetActive(true);

        StartCoroutine(ChangeVictoryTextsAfterDelay(bm1, bm2, bm3));
    }

    private IEnumerator ChangeVictoryTextsAfterDelay(string bm1, string bm2, string bm3)
    {
        yield return new WaitForSeconds(2f);

        victoryText.text = bm2;

        yield return new WaitForSeconds(2f);

        victoryText.text = bm3;
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

            StartCoroutine(LoadUpgradeScene());
            EndRoundRpc();
            ResetPlayers();
        }
    }

    private IEnumerator LoadUpgradeScene()
    {
        // Ensure the unload has completed
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);
        NetworkManager.SceneManager.LoadScene("UpgradeMap", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Everyone)]
    private void EndRoundRpc()
    {
        roundWinnerUI.SetActive(false);
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
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        StartCoroutine(NextMap());
    }

}
