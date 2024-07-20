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
    private const string G = "<color=#00FF00>";
    private const string XG = "</color>\n<color=#00FF00>";
    private const string R = "<color=#FF0000>";
    private const string XR = "</color>\n<color=#FF0000>";
    private const string X = "</color>";

    Dictionary<string, string> upgradeDictionary = new Dictionary<string, string>() {
    { "volley1", $"Volley Shot:\n {G} + Volley Fire Shape {XG} + 1 orb {XR} - Considerable Orb Damage {XR} - Modest Orb Cooldown {X}" },

    { "orbPower1", $"Orb Power:\n {G} + Considerable Orb Damage {XG} + Considerable Orb Knockback {XR} - More orb cooldown {X}" },

    { "orbKnockback1", $"Orb Knockback:\n {G} + More Orb Knockback {XR} - More orb cooldown {X}" },

    { "orbDamage1", $"Orb Damage:\n {G} + Huge Orb Damage {XR} - More orb cooldown {X}" },

    { "movement1", $"Run N Gun:\n {G} + Huge speed increase {X}" },

    { "homing", $"Homing:\n {G} + adds homing to your shots! {X}" }
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

    public void CallFunctionByName(string functionName)
    {
        if (!IsServer) return;
        Type type = this.GetType();
        MethodInfo methodInfo = type.GetMethod(functionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        
        if (methodInfo != null) methodInfo.Invoke(this, null);
        else Debug.LogError($"Method {functionName} not found in class {type.Name}");
    }

    private void volley1()
    {
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
        statsManager.orbDamage.Value += 0.5f;
        statsManager.orbKnockbackForce.Value += 10f;
        statsManager.orbKnockbackPercentDamage.Value += 0.08f;
        statsManager.orbPriority.Value += 1;
    }

    private void orbKnockback1()
    {
        statsManager.orbKnockbackForce.Value += 20f;
        statsManager.orbKnockbackPercentDamage.Value += 0.2f;
    }

    private void orbDamage1()
    {
        statsManager.orbDamage.Value += 1f;
        statsManager.orbPriority.Value += 2;
    }

    private void movement1()
    {
        statsManager.groundedMoveSpeed.Value += 2f;
        statsManager.airMoveSpeed.Value += 2f;
        statsManager.groundMultiplier.Value += 0.3f;
        statsManager.airMultiplier.Value += 0.3f;
    }


}
