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

    public Dictionary<string, string> upgradeDictionary = new Dictionary<string, string>() {
    { "volley1", $"Volley Shot:\n {G} + Volleyshot {XG} + 1 orb {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Modest orb cooldown {X}" },

    { "orbPower1", $"Orb Power:\n {G} + Considerable Orb Damage {XG} + Considerable Orb knockback {XR} - Modest orb cooldown {X}" },

    { "orbKnockback1", $"Orb Knockback:\n {G} + Huge orb Knockback {XR} - Modest orb cooldown {X}" },

    { "orbDamage1", $"Orb Damage:\n {G} + Huge Orb Damage {XR} - Modest orb cooldown {X}" },

    { "movement1", $"Run N' Gun:\n {G} + Huge Speed {X}" },

    { "homing1", $"Homing:\n {G} + Adds Orb Homing {XR} - Considerable orb speed {XR} - Modest orb damage {X}" },

    { "bounceshot1", $"Bounce Shot:\n {G} + Extra bounce towards enemy {XR} - Modest knockback {X}" },

    { "orbspeed1", $"orbspeed:\n {G} + Considerable orb speed {XR} - Modest orb cooldown {XR} - Small damage {XR} - Small knockback {X}" },

    { "burst1", $"Burst Shot:\n {G} + 1 orb burst {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Modest orb cooldown {X}" },

    { "explosion1", $"Explosion Power:\n {G} + Considerable explosion size {XG} + Modest explosion damage {XG} + Modest explosion knockback {XR} - Modest orb cooldown {X}" },

    { "orbsize1", $"Orb Size:\n {G} + Considerable orb size {XR} - Modest orb speed {XR} - small orb cooldown {X}" },

    { "cluster1", $"Cluster Shot:\n {G} + Clustershot {XG} + 1 orb {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Modest orb cooldown {X}" },

    { "multiDash1", $"Mutli Dash:\n {G} + 2 Additional Dash {X}" },

    { "dash1", $"Long Jump:\n {G} + Huge dash strength {X}" },

    { "health1", $"HP Increase:\n {G} + Huge HP {XR} - Small Speed {X}" },

    { "jump1", $"Extra Jump:\n {G} + 1 Jump {XG} + Modest Jump force {X} " },

    { "reload1", $"Quick Reload:\n {G} + Huge Cooldown {XR} - small knockback {XR} - small damage" },

    { "pulseknockback1", $"Melee Knockback:\n {G} + Huge Pulse Knockback {XR} - small orb Cooldown {XR} - small pulse cooldown {X}" },

    { "pulsedamage1", $"Melee Damage:\n {G} + Huge Pulse Damage {XR} - small orb Cooldown {XR} - small pulse cooldown {X}" },
    
    { "pulsecooldown1", $"Melee Cooldown:\n {G} + Huge Pulse Cooldown {X}" },

    { "topspeed1", $"Sanic:\n {G} + Huge top speed {XR} - Modest Agility {X}" },
    
    { "agility1", $"Dodging:\n {G} + Huge agililty {XR} - small HP {X}" },

    { "volley2", $"Volley Shot:\n {G} + Volleyshot {XG} + 2 orbs {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Huge orb cooldown {X}" },

    { "cluster2", $"Cluster Shot:\n {G} + Clustershot {XG} + 2 orbs {XR} - Considerable Orb damage {XR} - Considerable Orb Kockback {XR} - Huge orb cooldown {X}" },

    { "charge1", $"Charge Shot:\n {G} + Huge Max orb speed {XR} - Huge Charge time {X}" }
};

    //private bool triggered = false;

    //private Collider playerCollider;

    public PlayerStatsManager stats;


    // private void OnTriggerEnter(Collider collider)
    // {
    //     if(!IsServer || triggered) return;
    //     triggered = true;
        
    //     stats = collider.transform.root.GetComponent<PlayerStatsManager>();

    //     // List<string> upgradesToRemoveList = collider.transform.root.GetComponent<PlayerScript>().UpgradeList;
    //     // Debug.Log("upgradesToRemoveList is " + upgradesToRemoveList);
    //     // foreach(string upgrade in upgradesToRemoveList)
    //     // {
    //     //     Debug.Log("upgrade is  " + upgrade);
    //     //     upgradeDescriptions.RemoveAt(upgradeNames.IndexOf(upgrade));
    //     //     upgradeNames.Remove(upgrade);
    //     // }

    //     foreach(Transform child in transform)
    //     {
    //         if (upgradeDictionary.Count == 0) return;

    //         List<string> keys = new List<string>(upgradeDictionary.Keys);
    //         int index = UnityEngine.Random.Range(0, keys.Count);
    //         string upgradeName = keys[index];
    //         string upgradeDescription = upgradeDictionary[upgradeName];

    //         child.GetComponent<CardTrigger>().setUpgradeName(upgradeName);
    //         child.GetComponent<CardTrigger>().cardTextRpc(upgradeDescription);

    //         upgradeDictionary.Remove(upgradeName);
    //     }
    // }

    public void UpgradePlayer(string upgrade, PlayerStatsManager player)
    {
        if(!IsServer) return;

        stats = player;

        Type type = this.GetType();
        MethodInfo methodInfo = type.GetMethod(upgrade, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        
        if (methodInfo != null) methodInfo.Invoke(this, null);
        else Debug.LogError($"Method {upgrade} not found in class {type.Name}");
    }

    private void volley1()
    {
        stats.fireShape.Value = "volley";
        stats.numberOfOrbs.Value += 1;
        stats.orbDamage.Value *= 0.8f;
        stats.orbKnockbackForce.Value *= 0.8f;
        stats.orbKnockbackPercentDamage.Value *= 0.8f;
        stats.orbCooldown.Value += 0.2f;
        stats.orbPriority.Value -= 1;
    }

    private void orbPower1()
    {
        stats.orbDamage.Value += 1f;
        stats.orbKnockbackForce.Value += 10f;
        stats.orbKnockbackPercentDamage.Value += 0.2f;
        stats.orbPriority.Value += 1;
    }

    private void orbKnockback1()
    {
        stats.orbKnockbackForce.Value += 25f;
        stats.orbKnockbackPercentDamage.Value += 0.4f;
    }

    private void orbDamage1()
    {
        stats.orbDamage.Value += 1.5f;
        stats.orbPriority.Value += 2;
    }

    private void movement1()
    {
        stats.groundedMoveSpeed.Value += 4f;
        stats.airMoveSpeed.Value += 4f;
        stats.groundMultiplier.Value += 0.6f;
        stats.airMultiplier.Value += 0.6f;
    }

    private void homing1()
    {
        stats.homing.Value += 30f;
        stats.orbMinSpeed.Value -= 10f;
        stats.orbMaxSpeed.Value -= 20f;
        stats.orbDamage.Value -= 0.5f;
    }

    private void bounceshot1()
    {
        stats.bounces.Value += 1;
        stats.orbKnockbackForce.Value -= 15f;
        stats.orbKnockbackPercentDamage.Value -= 0.05f;
    }

    private void orbspeed1()
    {
        stats.orbMinSpeed.Value += 25f;
        stats.orbMaxSpeed.Value += 35f;
        stats.orbDamage.Value -= 0.1f;
        stats.orbKnockbackForce.Value -= 5f;
        stats.orbKnockbackPercentDamage.Value -= 0.025f;
        stats.orbPriority.Value += 2;
    }

    private void burst1()
    {
        stats.orbBurst.Value += 1;
        stats.orbKnockbackForce.Value -= 15f;
        stats.orbKnockbackPercentDamage.Value -= 0.05f;
        stats.orbDamage.Value *= 0.8f;
        stats.orbKnockbackForce.Value *= 0.8f;
        stats.orbKnockbackPercentDamage.Value *= 0.8f;
        stats.orbCooldown.Value += 0.2f;
        stats.orbPriority.Value -= 1;
    }

    private void explosion1()
    {
        stats.explosionRadius.Value += 1.5f;
        stats.explosionDamage.Value += 0.5f;
        stats.explosionKnockbackForce.Value += 10f;
        stats.explosionKnockbackPercentDamage.Value += 0.05f;
        stats.orbCooldown.Value += 0.1f;
    }

    private void orbsize1()
    {
        stats.orbScale.Value += 0.3f;
        stats.orbMinSpeed.Value -= 5f;
        stats.orbMaxSpeed.Value -= 10f;
        stats.orbCooldown.Value += 0.1f;
    }

    private void cluster1()
    {
        stats.fireShape.Value = "cluster";
        stats.numberOfOrbs.Value += 1;
        stats.orbDamage.Value *= 0.8f;
        stats.orbKnockbackForce.Value *= 0.8f;
        stats.orbKnockbackPercentDamage.Value *= 0.8f;
        stats.orbCooldown.Value += 0.2f;
        stats.orbPriority.Value -= 1;
    }

    private void multiDash1()
    {
        stats.numberOfDashes.Value += 2;
    }

    private void dash1()
    {
        stats.dashForce.Value += 25f;
    }

    private void health1()
    {
        stats.maxPlayerHealth.Value += 10f;
        stats.groundedMoveSpeed.Value -= 1f;
        stats.airMoveSpeed.Value -= 1f;
        stats.groundMultiplier.Value -= 0.2f;
        stats.airMultiplier.Value -= 0.2f;
    }

    private void jump1()
    {
        stats.numberOfJumps.Value += 1;
        stats.jumpForce.Value += 5f;
    }

    private void reload1()
    {
        stats.orbCooldown.Value -= 0.5f;
        stats.orbChargeTime.Value -= 0.5f;
        stats.orbDamage.Value -= 0.3f;
    }

    private void pulsedamage1()
    {
        stats.pulseDamage.Value += 1.5f;
        stats.pulseCooldown.Value += 0.4f;
        stats.orbCooldown.Value += 0.1f;

    }

    private void pulseknockback1()
    {
        stats.pulseKnockbackForce.Value += 15f;
        stats.pulseKnockbackPercentDamage.Value += 0.3f;
        stats.pulseCooldown.Value += 0.4f;
        stats.orbCooldown.Value += 0.1f;
    }

    private void pulsecooldown1()
    {
        stats.pulseCooldown.Value -= 1f;
    }

    private void topspeed1()
    {
        stats.groundedMoveSpeed.Value += 8f;
        stats.airMoveSpeed.Value += 8f;
        stats.groundMultiplier.Value -= 0.2f;
        stats.airMultiplier.Value -= 0.2f;
    }

    private void agility1()
    {
        stats.groundMultiplier.Value += 1.2f;
        stats.airMultiplier.Value += 1.2f;
        stats.maxPlayerHealth.Value -= 2f;
    }

    private void volley2()
    {
        stats.fireShape.Value = "volley";
        stats.numberOfOrbs.Value += 2;
        stats.orbDamage.Value *= 0.6f;
        stats.orbKnockbackForce.Value *= 0.6f;
        stats.orbKnockbackPercentDamage.Value *= 0.6f;
        stats.orbCooldown.Value += 1f;
        stats.orbPriority.Value -= 2;
    }

    private void cluster2()
    {
        stats.fireShape.Value = "cluster";
        stats.numberOfOrbs.Value += 2;
        stats.orbDamage.Value *= 0.6f;
        stats.orbKnockbackForce.Value *= 0.6f;
        stats.orbKnockbackPercentDamage.Value *= 0.6f;
        stats.orbCooldown.Value += 1f;
        stats.orbPriority.Value -= 2;
    }

    private void charge1()
    {
        stats.orbMaxSpeed.Value += 60f;
        stats.orbChargeTime.Value += 1.5f;
        stats.orbPriority.Value += 1;
    }


}
