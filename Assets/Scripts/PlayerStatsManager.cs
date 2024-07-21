using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System;

public class PlayerStatsManager : NetworkBehaviour
{

    void Start()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        PlayerStatsManager player = this;

        gameManager.GetComponent<UpgradeManager>().UpgradePlayer("explosion1", player);
        GetComponent<PlayerScript>().UpgradeList.Add("explosion1");

        gameManager.GetComponent<UpgradeManager>().UpgradePlayer("orbspeed1", player);
        GetComponent<PlayerScript>().UpgradeList.Add("orbspeed1");

        gameManager.GetComponent<UpgradeManager>().UpgradePlayer("bounceshot1", player);
        GetComponent<PlayerScript>().UpgradeList.Add("bounceshot1");
    }

    // ------------------------------- Player Health Stuff -------------------------------

    [NonSerialized] public NetworkVariable<float> playerHealth = new NetworkVariable<float>(20f);

    [NonSerialized] public NetworkVariable<float> maxPlayerHealth = new NetworkVariable<float>(20f);
    
    [NonSerialized] public NetworkVariable<float> knockbackBuildUp = new NetworkVariable<float>(1f);

    


    // ------------------------------- Orb Related Stuff -------------------------------

    [NonSerialized] public NetworkVariable<float> orbDamage = new NetworkVariable<float>(2f);

    [NonSerialized] public NetworkVariable<float> orbKnockbackForce = new NetworkVariable<float>(50f);

    [NonSerialized] public NetworkVariable<float> orbKnockbackPercentDamage = new NetworkVariable<float>(0.2f);

    [NonSerialized] public NetworkVariable<float> orbCooldown = new NetworkVariable<float>(1.3f);

    [NonSerialized] public NetworkVariable<float> orbMaxSpeed = new NetworkVariable<float>(100f);

    [NonSerialized] public NetworkVariable<float> orbMinSpeed = new NetworkVariable<float>(50f);

    [NonSerialized] public NetworkVariable<float> orbChargeTime = new NetworkVariable<float>(1.2f);

    [NonSerialized] public NetworkVariable<float> orbScale = new NetworkVariable<float>(0.8f);

    [NonSerialized] public NetworkVariable<int> orbPriority = new NetworkVariable<int>(1);

    // ------------------------------- Explosion Related Stuff -------------------------------
    [NonSerialized] public NetworkVariable<float> explosionDamage = new NetworkVariable<float>(1f);

    [NonSerialized] public NetworkVariable<float> explosionKnockbackForce = new NetworkVariable<float>(12f);

    [NonSerialized] public NetworkVariable<float> explosionKnockbackPercentDamage = new NetworkVariable<float>(0.1f);

    [NonSerialized] public NetworkVariable<float> explosionRadius = new NetworkVariable<float>(4f);


    // ------------------------------- Block/Pulse Related Stuff -------------------------------
    [NonSerialized] public NetworkVariable<float> pulseDamage = new NetworkVariable<float>(3.5f);

    [NonSerialized] public NetworkVariable<float> pulseKnockbackForce = new NetworkVariable<float>(30f);

    [NonSerialized] public NetworkVariable<float> pulseKnockbackPercentDamage = new NetworkVariable<float>(0.05f);

    [NonSerialized] public NetworkVariable<float> pulseRadius = new NetworkVariable<float>(4f);

    [NonSerialized] public NetworkVariable<float> pulseCooldown = new NetworkVariable<float>(2.5f);




    // ------------------------------- Movement Related Stuff -------------------------------
    [NonSerialized] public NetworkVariable<int> numberOfJumps = new NetworkVariable<int>(2);

    [NonSerialized] public NetworkVariable<int> numberOfDashes = new NetworkVariable<int>(1);

    [NonSerialized] public NetworkVariable<float> jumpForce = new NetworkVariable<float>(16f);

    [NonSerialized] public NetworkVariable<float> dashForce = new NetworkVariable<float>(28f);

    [NonSerialized] public NetworkVariable<float> dashCooldown = new NetworkVariable<float>(1.3f);

    [NonSerialized] public NetworkVariable<float> groundedMoveSpeed = new NetworkVariable<float>(8f);

    [NonSerialized] public NetworkVariable<float> airMoveSpeed = new NetworkVariable<float>(5f);

    [NonSerialized] public NetworkVariable<float> groundMultiplier = new NetworkVariable<float>(10f);

    [NonSerialized] public NetworkVariable<float> airMultiplier = new NetworkVariable<float>(15f);
        // other movement vars

    

    // ------------------------------- Altered Mechanics Related Stuff -------------------------------

    [NonSerialized] public NetworkVariable<int> numberOfOrbs = new NetworkVariable<int>(1);
    
    [NonSerialized] public NetworkVariable<FixedString32Bytes> fireShape = new NetworkVariable<FixedString32Bytes>("default");

    [NonSerialized] public NetworkVariable<float> homing = new NetworkVariable<float>(0f);

    [NonSerialized] public NetworkVariable<int> orbBurst = new NetworkVariable<int>(1);

    [NonSerialized] public NetworkVariable<float> orbBurstDelay = new NetworkVariable<float>(0.1f);

    [NonSerialized] public NetworkVariable<int> bounces = new NetworkVariable<int>(0);


}
