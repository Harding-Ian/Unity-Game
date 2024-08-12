using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreOverlay : MonoBehaviour
{
    public GameObject PlayerScores;
    private int numberOfPlayers;

    public GameObject GameManager;

    void Start()
    {
        //GameManager = GameObject.Find("GameManager");
    }

    void OnEnable()
    {
        numberOfPlayers = getNumberofPlayers();
        //Debug.Log("numPlayers === " + numberOfPlayers);
        enableScoreBars();
    }

    void OnDisable()
    {
        DisableScoreBars();
    }


    private void enableScoreBars()
    {
        List<float> scoreBarPositions = getScoreBarPositions();
        List<GameObject> players = getPlayers();

        int maxWins = GameManager.GetComponent<GameSceneManager>().winCondition.Value;

        int i = 0;
        foreach(Transform child in PlayerScores.transform)
        {
            if(i >= numberOfPlayers) return;

            child.gameObject.SetActive(true);
            //Debug.Log("setting position of child " + i + " to " + scoreBarPositions[i]);
            child.GetComponent<RectTransform>().localPosition = new Vector3(child.GetComponent<RectTransform>().localPosition.x, scoreBarPositions[i], child.GetComponent<RectTransform>().localPosition.z);
            //Debug.Log("player " + i + " has " + playerScores[i] + " wins and maxwins is " + maxWins); 
            child.GetComponent<Slider>().value = (float) players[i].GetComponent<PlayerScript>().wins.Value / (float) maxWins;
            child.Find("Fill").GetComponent<Image>().color = players[i].transform.Find("Model/Body").GetComponent<Renderer>().materials[0].color;

            i++;
        }
    }

    private void DisableScoreBars()
    {
        foreach(Transform child in PlayerScores.transform)
        {
            child.gameObject.SetActive(false);
        }
    }


    private List<GameObject> getPlayers()
    {
        List<GameObject> players = new List<GameObject>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.InstanceID))
        {
            //Debug.Log("player " + instance.OwnerClientId + " has " + instance.wins.Value + " wins");
            players.Add(instance.gameObject);
        }
        return players;
    }


    private int getNumberofPlayers()
    {
        int i = 0;
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None)) i++;
        return i;
    }


    private List<float> getScoreBarPositions()
    {
        List<float> scoreBarPositions = new List<float>();

        float separation = 35f;

        float BarPosition =  ((numberOfPlayers - 1f) / 2f) * separation;

        for(int i = 0; i < numberOfPlayers; i++)
        {
            scoreBarPositions.Add(BarPosition);
            //Debug.Log(BarPosition + " added to scoreBarPositions");
            BarPosition = BarPosition - separation; 
        }
        return scoreBarPositions;
    }
}
