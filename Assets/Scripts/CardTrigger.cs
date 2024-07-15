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
    private UpgradeManager upgradeManager;

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


    private void OnTriggerEnter(Collider collider){
        if (IsServer && collider.transform.root.GetComponent<PlayerScript>().upgraded.Value == false){
            collider.transform.root.GetComponent<PlayerScript>().upgraded.Value = true;
            if (upgradeName == "empty"){
                Debug.Log("No more upgrades left");
                return;
            }
            Debug.Log("upgrade acquired!");
            upgradeManager.GetComponent<UpgradeManager>().CallFunctionByName(upgradeName);
            collider.transform.root.GetComponent<PlayerScript>().UpgradeList.Add(upgradeName);

            gameManager.GetComponent<GameSceneManager>().checkAllUpgraded();
        }
    }

}

