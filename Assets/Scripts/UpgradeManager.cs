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
    { "volley1", $"Volley Shot:\n {G} + Volleyshot {XG} + 1 orb {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Modest orb cooldown {X}" },

    { "orbPower1", $"Orb Power:\n {G} + Considerable Orb Damage {XG} + Considerable Orb knockback {XR} - Modest orb cooldown {X}" },

    { "orbKnockback1", $"Orb Knockback:\n {G} + Huge orb Knockback {XR} - Modest orb cooldown {X}" },

    { "orbDamage1", $"Orb Damage:\n {G} + Huge Orb Damage {XR} - Modest orb cooldown {X}" },

    { "movement1", $"Run N' Gun:\n {G} + Huge speed increase {X}" },

    { "homing1", $"Homing:\n {G} + adds homing to your shots! {XR} - Considerable orb speed {XR} - Modest orb damage {X}" },

    { "bounceshot1", $"Bounce Shot:\n {G} + adds an extra bounce towards the nearest player! {XR} - Modest knockback {X}" },

    { "orbspeed1", $"orbspeed:\n {G} + Considerable orb speed {XR} - Modest orb cooldown {XR} - Small damage {XR} - Small knockback {X}" },

    { "burst1", $"Burst Shot:\n {G} + 1 orb burst {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Modest orb cooldown {X}" },

    { "explosion1", $"Explosion Power:\n {G} + Considerable explosion size {XG} + Modest explosion damage {XG} + Modest explosion knockback {XR} - Modest orb cooldown {X}" },

    { "orbsize1", $"Orb Size:\n {G} + Considerable orb size {XR} - Modest orb speed {XR} - small orb cooldown {X}" }
/*














*/


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

    private void homing1()
    {
        statsManager.homing.Value += 10f;
        statsManager.orbMinSpeed.Value -= 20f;
        statsManager.orbMaxSpeed.Value -= 30f;
        statsManager.orbDamage.Value -= 0.5f;
    }

    private void bounceshot1()
    {
        statsManager.bounces.Value += 1;
        statsManager.orbKnockbackForce.Value -= 15f;
        statsManager.orbKnockbackPercentDamage.Value -= 0.05f;
    }

    private void orbspeed1()
    {
        statsManager.orbMinSpeed.Value += 25f;
        statsManager.orbMaxSpeed.Value += 35f;
        statsManager.orbCooldown.Value -= 0.2f;
        statsManager.orbDamage.Value -= 0.1f;
        statsManager.orbKnockbackForce.Value -= 5f;
        statsManager.orbKnockbackPercentDamage.Value -= 0.025f;
        statsManager.orbPriority.Value += 2;
    }

    private void burst1()
    {
        statsManager.orbBurst.Value += 1;
        statsManager.orbKnockbackForce.Value -= 15f;
        statsManager.orbKnockbackPercentDamage.Value -= 0.05f;
        statsManager.orbDamage.Value *= 0.8f;
        statsManager.orbKnockbackForce.Value *= 0.8f;
        statsManager.orbKnockbackPercentDamage.Value *= 0.8f;
        statsManager.orbCooldown.Value += 0.2f;
        statsManager.orbPriority.Value -= 1;
    }

    private void explosion1()
    {
        statsManager.explosionRadius.Value += 1.5f;
        statsManager.explosionDamage.Value += 0.5f;
        statsManager.explosionKnockbackForce.Value += 10f;
        statsManager.explosionKnockbackPercentDamage.Value += 0.05f;
        statsManager.orbCooldown.Value -= 0.2f;
    }

    private void orbsize1()
    {
        statsManager.orbScale.Value += 0.3f;
        statsManager.orbMinSpeed.Value -= 10f;
        statsManager.orbMaxSpeed.Value -= 15f;
        statsManager.orbCooldown.Value -= 0.1f;
    }

}
