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

    void Start()
    {
        gameManager = GameObject.Find("GameManager");
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

