using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Unity.Netcode;

public class UpgradeManager : NetworkBehaviour
{
    List<string> upgradeNames = new List<string>() {"test1", "test2", "test3", "test4", "test5"};
    List<string> upgradeDescriptions = new List<string>() {"desc1", "desc2", "desc3", "desc4", "desc5"};

    private bool triggered = false;


    private void OnTriggerEnter(Collider collider)
    {
        if(!IsServer || triggered) return;
        triggered = true;

        List<string> upgradesToRemoveList = collider.GetComponent<PlayerScript>().UpgradeList;
        Debug.Log("upgradesToRemoveList is " + upgradesToRemoveList);
        foreach(string upgrade in upgradesToRemoveList)
        {
            Debug.Log("upgrade is  " + upgrade);
            upgradeDescriptions.RemoveAt(upgradeNames.IndexOf(upgrade));
            upgradeNames.Remove(upgrade);
        }

        foreach(Transform child in transform)
        {
            if (upgradeNames.Count == 0){
                return;
            }
            int index = UnityEngine.Random.Range(0, upgradeNames.Count);
            child.GetComponent<CardTrigger>().setUpgradeName(upgradeNames[index]);
            child.GetComponent<CardTrigger>().cardTextRpc(upgradeDescriptions[index]);
            upgradeNames.RemoveAt(index);
            upgradeDescriptions.RemoveAt(index);
        }
    }

    private void test1()
    {
        Debug.Log("test1");
    }

    private void test2()
    {
        Debug.Log("test2");
    }

    private void test3()
    {
        Debug.Log("test3");
    }

    private void test4()
    {
        Debug.Log("test4");
    }

    private void test5()
    {
        Debug.Log("test5");
    }


    public void CallFunctionByName(string functionName)
    {
        if (!IsServer) return;
        Type type = this.GetType();
        MethodInfo methodInfo = type.GetMethod(functionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        
        if (methodInfo != null)
        {
            methodInfo.Invoke(this, null);
        }
        else
        {
            Debug.LogError($"Method {functionName} not found in class {type.Name}");
        }
    }

}
