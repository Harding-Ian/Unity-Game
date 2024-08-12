using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : NetworkBehaviour
{

    [SerializeField]
    public GameObject victoryUI;

    [SerializeField]
    private TextMeshProUGUI victoryText;
    
    [SerializeField]
    private TextMeshProUGUI victoryName;

    public GameObject ScreenUI;
    public bool inEndGameScreen = false;

    [Rpc(SendTo.Everyone)]
    public void GameCompletedRpc(string winner, string bm1, string bm2, string bm3)
    {
        victoryText.text = bm1;
        victoryName.text = winner + " Wins";
        victoryUI.SetActive(true);

        StartCoroutine(ChangeVictoryTextsAfterDelay(bm1, bm2, bm3));
        Invoke(nameof(EndGame), 8f);
    }

    private IEnumerator ChangeVictoryTextsAfterDelay(string bm1, string bm2, string bm3)
    {
        yield return new WaitForSeconds(2f);

        victoryText.text = bm2;

        yield return new WaitForSeconds(2f);

        victoryText.text = bm3;
    }

    private void EndGame()
    {
        
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        EndGameScreenRpc();

        StartCoroutine(LoadEndGameScreen());
    }

    [Rpc(SendTo.Everyone)]
    public void EndGameScreenRpc()
    {
        inEndGameScreen = true;
        victoryUI.SetActive(false);
        ScreenUI.SetActive(false);
        if(IsHost)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private IEnumerator LoadEndGameScreen()
    {
        yield return new WaitUntil(() => !SceneManager.GetSceneAt(SceneManager.sceneCount - 1).isLoaded);
        NetworkManager.SceneManager.LoadScene("EndMenu", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.Everyone)]
    public void ExitEndGameScreenRpc()
    {
        inEndGameScreen = false;
        ScreenUI.SetActive(true);
        if(IsHost)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
