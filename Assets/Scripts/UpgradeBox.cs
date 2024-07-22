using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UpgradeBox : NetworkBehaviour
{

    private bool triggered = false;

    GameObject gameManager;

    Dictionary<string, string> upgradeDictionary;


    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        //upgradeDictionary = gameManager.GetComponent<UpgradeManager>().upgradeDictionary;
        upgradeDictionary = new Dictionary<string, string>(gameManager.GetComponent<UpgradeManager>().upgradeDictionary);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(!IsServer || triggered) return;
        triggered = true;

        // List<string> upgradesToRemoveList = collider.transform.root.GetComponent<PlayerScript>().UpgradeList;
        // Debug.Log("upgradesToRemoveList is " + upgradesToRemoveList);
        // foreach(string upgrade in upgradesToRemoveList)
        // {
        //     Debug.Log("upgrade is  " + upgrade);
        //     upgradeDescriptions.RemoveAt(upgradeNames.IndexOf(upgrade));
        //     upgradeNames.Remove(upgrade);
        // }

        foreach(Transform child in transform)
        {
            if (upgradeDictionary.Count == 0) return;

            List<string> keys = new List<string>(upgradeDictionary.Keys);
            int index = UnityEngine.Random.Range(0, keys.Count);
            string upgradeName = keys[index];
            string upgradeDescription = upgradeDictionary[upgradeName];

            child.GetComponent<CardTrigger>().setUpgradeName(upgradeName);
            child.GetComponent<CardTrigger>().cardTextRpc(upgradeDescription);

            upgradeDictionary.Remove(upgradeName);
        }
    }



}
