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

    [SerializeField]
    private GameObject cardText;

    private string upgradeName = "empty";

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
        if (IsServer && collider.gameObject.GetComponent<PlayerScript>().upgraded.Value == false){
            collider.gameObject.GetComponent<PlayerScript>().upgraded.Value = true;
            if (upgradeName == "empty"){
                Debug.Log("No more upgrades left");
                return;
            }
            Debug.Log("upgrade acquired!");
            upgradeManager.GetComponent<UpgradeManager>().CallFunctionByName(upgradeName);
            collider.GetComponent<PlayerScript>().UpgradeList.Add(upgradeName);
        }
    }

}

