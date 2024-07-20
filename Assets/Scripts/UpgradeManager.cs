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


    Dictionary<string, string> upgradeDictionary = new Dictionary<string, string>() {
    { "volley1", "Volley Shot:\n <color=#00FF00> + Volley Fire Shape </color>\n <color=#00FF00> + 1 orb </color>\n <color=#FF0000> - Considerable Orb Damage </color>\n <color=#FF0000> - Modest Orb Cooldown </color>" },

    { "orbPower1", "Orb Power:\n <color=#009900> + Considerable Orb Damage </color>\n <color=#009900> + Considerable Orb Knockback </color>\n <color=#cf0000> - More orb cooldown </color>" },

    { "orbKnockback1", "Orb Knockback:\n <color=#00FF00> + More Orb Knockback </color>\n <color=#FF0000> - More orb cooldown </color>" },

    { "orbDamage1", "Orb Damage:\n <color=#00FF00> + Huge Orb Damage </color>\n <color=#FF0000> - More orb cooldown </color>" },

    { "movement1", "Run N Gun:\n <color=#00FF00> + Huge speed increase" }
};

    private bool triggered = false;

    private Collider playerCollider;

    private PlayerStatsManager statsManager;


    private void OnTriggerEnter(Collider collider)
    {
        if(!IsServer || triggered) return;
        triggered = true;
        
        statsManager = collider.transform.root.GetComponent<PlayerStatsManager>();

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

    private void volley1()
    {
        Debug.Log("volley1 upgrade selected");

        statsManager.fireShape.Value = "volley";
        statsManager.numberOfOrbs.Value += 1;
        statsManager.orbDamage.Value *= 0.8f;
        statsManager.orbKnockbackForce.Value *= 0.8f;
        statsManager.orbKnockbackPercentDamage.Value *= 0.8f;
        statsManager.orbCooldown.Value += 0.2f;
        statsManager.orbPriority.Value -= 1;

    }

    private void cluster1()
    {
        Debug.Log("volley1 upgrade selected");

        statsManager.fireShape.Value = "cluster";
        statsManager.numberOfOrbs.Value += 1;
        statsManager.orbDamage.Value *= 0.8f;
        statsManager.orbKnockbackForce.Value *= 0.8f;
        statsManager.orbKnockbackPercentDamage.Value *= 0.8f;
        statsManager.orbCooldown.Value += 0.2f;
        statsManager.orbPriority.Value -= 1;
    }

    private void orbPower1()
    {
        Debug.Log("orbPower1 upgrade selected");

        statsManager.orbDamage.Value += 0.5f;
        statsManager.orbKnockbackForce.Value += 10f;
        statsManager.orbKnockbackPercentDamage.Value += 0.08f;
        statsManager.orbPriority.Value += 1;
    
    }

    private void orbKnockback1()
    {
        Debug.Log("orb knockback1 upgrade selected");

        statsManager.orbKnockbackForce.Value += 20f;
        statsManager.orbKnockbackPercentDamage.Value += 0.2f;
    }

    private void orbDamage1()
    {
        Debug.Log("orb damage 1 upgrade selected");
        statsManager.orbDamage.Value += 1f;
        statsManager.orbPriority.Value += 2;
    }

    private void movement1()
    {
        Debug.Log("movement 1 upgrade selected");
        statsManager.groundedMoveSpeed.Value += 2f;
        statsManager.airMoveSpeed.Value += 2f;
        statsManager.groundMultiplier.Value += 0.3f;
        statsManager.airMultiplier.Value += 0.3f;
    }


}
