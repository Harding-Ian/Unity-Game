using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Unity.Netcode;

public class UpgradeManager : NetworkBehaviour
{

    // Small, Modest, Considerable, Huge, Colossal
    // 1/5    2/5     3/5           4/5   5/5

    // 
    // Dictionary<string, string> upgradeDictionary = new Dictionary<string, string>() {
    // { "volley1", "Volley Shot:\n <color=#00FF00> + Volley Fire Shape </color>\n <color=#00FF00> + 1 orb </color>\n <color=#FF0000> - Considerable Orb Damage </color>\n <color=#FF0000> - Modest Orb Cooldown </color>" },

    // { "cluster1", "Cluster Shot:\n <color=#00FF00> + Cluster Fire Shape </color>\n <color=#00FF00> + 1 orb </color>\n <color=#FF0000> - Considerable Orb Damage Decrease </color> \n <color=#FF0000> - Modest Orb Cooldown </color>" },

    // { "orbPower1", "Orb Power:\n <color=#009900> + Huge Orb Damage </color>\n <color=#cf0000> - More orb cooldown </color>" },

    // { "orbKnockback1", "Orb Knockback:\n <color=#00FF00> + More Orb Knockback </color>\n <color=#FF0000> - More orb cooldown </color>" }

    Dictionary<string, string> upgradeDictionary = new Dictionary<string, string>() {
    { "volley1", "Volley Shot:\n <color=#00FF00> + Volley Fire Shape </color>\n <color=#00FF00> + 1 orb </color>\n <color=#FF0000> - Considerable Orb Damage </color>\n <color=#FF0000> - Modest Orb Cooldown </color>" },

    { "orbPower1", "Orb Power:\n <color=#009900> + Considerable Orb Damage </color>\n <color=#009900> + Considerable Orb Knockback </color>\n <color=#cf0000> - More orb cooldown </color>" },

    { "orbKnockback1", "Orb Knockback:\n <color=#00FF00> + More Orb Knockback </color>\n <color=#FF0000> - More orb cooldown </color>" },

    { "orbDamage1", "Orb Damage:\n <color=#00FF00> + Huge Orb Damage </color>\n <color=#FF0000> - More orb cooldown </color>" }
};

    private bool triggered = false;

    private Collider playerCollider;


    private void OnTriggerEnter(Collider collider)
    {
        if(!IsServer || triggered) return;
        triggered = true;
        
        playerCollider = collider;

        // List<string> upgradesToRemoveList = collider.GetComponent<PlayerScript>().UpgradeList;
        // Debug.Log("upgradesToRemoveList is " + upgradesToRemoveList);
        // foreach(string upgrade in upgradesToRemoveList)
        // {
        //     Debug.Log("upgrade is  " + upgrade);
        //     upgradeDescriptions.RemoveAt(upgradeNames.IndexOf(upgrade));
        //     upgradeNames.Remove(upgrade);
        // }

        foreach(Transform child in transform)
        {
            if (upgradeDictionary.Count == 0){
                return;
            }

            List<string> keys = new List<string>(upgradeDictionary.Keys);
            int index = UnityEngine.Random.Range(0, keys.Count);
            string upgradeName = keys[index];
            string upgradeDescription = upgradeDictionary[upgradeName];

            child.GetComponent<CardTrigger>().setUpgradeName(upgradeName);
            child.GetComponent<CardTrigger>().cardTextRpc(upgradeDescription);

            upgradeDictionary.Remove(upgradeName);
        }
    }

    private void volley1()
    {
        Debug.Log("volley1 upgrade selected");

        playerCollider.GetComponent<PlayerStatsManager>().fireShape.Value = "volley";
        playerCollider.GetComponent<PlayerStatsManager>().numberOfOrbs.Value += 1;
        playerCollider.GetComponent<PlayerStatsManager>().orbDamage.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackForce.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackPercentDamage.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbCooldown.Value += 0.2f;

        //reduce knockback
        //reduce knockback buildup
        //playerCollider.GetComponent<PlayerStatsManager>().
    }

    private void cluster1()
    {
        Debug.Log("volley1 upgrade selected");

        playerCollider.GetComponent<PlayerStatsManager>().fireShape.Value = "cluster";
        playerCollider.GetComponent<PlayerStatsManager>().numberOfOrbs.Value += 1;
        playerCollider.GetComponent<PlayerStatsManager>().orbDamage.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackForce.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackPercentDamage.Value *= 0.8f;
        playerCollider.GetComponent<PlayerStatsManager>().orbCooldown.Value += 0.2f;
    }

    private void orbPower1()
    {
        Debug.Log("orbPower1 upgrade selected");

        playerCollider.GetComponent<PlayerStatsManager>().orbDamage.Value += 0.5f;
        playerCollider.GetComponent<PlayerStatsManager>().orbCooldown.Value += 0.4f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackForce.Value += 10f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackPercentDamage.Value += 0.08f;
    
    }

    private void orbKnockback1()
    {
        Debug.Log("orb knockback1 upgrade selected");

        playerCollider.GetComponent<PlayerStatsManager>().orbCooldown.Value += 0.4f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackForce.Value += 20f;
        playerCollider.GetComponent<PlayerStatsManager>().orbKnockbackPercentDamage.Value += 0.2f;
    }

    private void orbDamage1()
    {
        Debug.Log("orb damage 1 upgrade selected");
        playerCollider.GetComponent<PlayerStatsManager>().orbDamage.Value += 1f;
        playerCollider.GetComponent<PlayerStatsManager>().orbCooldown.Value += 0.4f;
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
