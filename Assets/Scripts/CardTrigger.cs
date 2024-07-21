using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;

public class CardTrigger : NetworkBehaviour
{

    [SerializeField]

    private GameObject gameManager;

    [SerializeField]
    private GameObject cardText;

    private string upgradeName = "empty";

    private Vector3 originalPosition;

    public Vector2 hoverHeightRange = new Vector2(0.6f, 0.8f);
    public Vector2 hoverDurationRange = new Vector2(5f, 12f);

    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        originalPosition = transform.position;
        StartCoroutine(Hover());
    }

    IEnumerator Hover()
    {
        while (true)
        {
            float hoverUpDuration = Random.Range(hoverDurationRange.x, hoverDurationRange.y);

            float hoverHeight = Random.Range(hoverHeightRange.x, hoverHeightRange.y);

            yield return StartCoroutine(MoveToPosition(originalPosition + Vector3.up * hoverHeight, hoverUpDuration));

            float hoverDownDuration = Random.Range(hoverDurationRange.x, hoverDurationRange.y);

            yield return StartCoroutine(MoveToPosition(originalPosition, hoverDownDuration));
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    public void setUpgradeName(string name)
    {
        upgradeName = name;
    }


    [Rpc(SendTo.Everyone)]
    public void cardTextRpc(string textInput){
        if(upgradeName == "empty"){
            cardText.GetComponent<TextMeshProUGUI>().text = "Error... Oopsies";
        }
        cardText.GetComponent<TextMeshProUGUI>().text = textInput;
    }


    private void OnTriggerEnter(Collider collider)
    {
        PlayerScript player = collider.transform.root.GetComponent<PlayerScript>();
        if (IsServer && player.upgraded.Value == false)
        {
            player.upgraded.Value = true;
            if (upgradeName == "empty") return;

            gameManager.GetComponent<UpgradeManager>().UpgradePlayer(upgradeName, collider.transform.root.GetComponent<PlayerStatsManager>());
            player.UpgradeList.Add(upgradeName);

            gameManager.GetComponent<GameSceneManager>().checkAllUpgraded();
        }
    }

}

