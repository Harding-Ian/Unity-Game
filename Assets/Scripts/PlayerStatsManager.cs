using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

public class PlayerStatsManager : NetworkBehaviour
{



    // ------------------------------- Player Health Stuff -------------------------------

    public NetworkVariable<float> playerHealth = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> maxPlayerHealth = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<float> knockbackBuildUp = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    



    // ------------------------------- Orb Related Stuff -------------------------------

    public NetworkVariable<float> orbDamage = new NetworkVariable<float>(2f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> orbKnockbackForce = new NetworkVariable<float>(50f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> orbKnockbackPercentDamage = new NetworkVariable<float>(0.2f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> orbCooldown = new NetworkVariable<float>(1.3f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> maxProjectileSpeed = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> minProjectileSpeed = new NetworkVariable<float>(50f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);   

    public NetworkVariable<float> projectileChargeTime = new NetworkVariable<float>(1.2f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // ------------------------------- Explosion Related Stuff -------------------------------
    public NetworkVariable<float> explosionDamage = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> explosionKnockbackForce = new NetworkVariable<float>(12f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> explosionKnockbackPercentDamage = new NetworkVariable<float>(0.1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> explosionRadius = new NetworkVariable<float>(4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // ------------------------------- Block/Pulse Related Stuff -------------------------------
    public NetworkVariable<float> pulseDamage = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//

    public NetworkVariable<float> pulseKnockbackForce = new NetworkVariable<float>(30f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//

    public NetworkVariable<float> pulseKnockbackPercentDamage = new NetworkVariable<float>(0.05f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//

    public NetworkVariable<float> pulseRadius = new NetworkVariable<float>(4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);//

    public NetworkVariable<float> pulseCooldown = new NetworkVariable<float>(2.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);




    // ------------------------------- Movement Related Stuff -------------------------------

    public NetworkVariable<int> numberOfJumps = new NetworkVariable<int>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> numberOfDashes = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> jumpForce = new NetworkVariable<float>(12f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> dashForce = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> dashCooldown = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
        // other movement vars


    

    // ------------------------------- Altered Mechanics Related Stuff -------------------------------

    public NetworkVariable<int> numberOfOrbs = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<FixedString32Bytes> fireShape = new NetworkVariable<FixedString32Bytes>("default", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); 

}
