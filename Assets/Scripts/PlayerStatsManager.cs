using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour
{

    public NetworkVariable<float> playerHealth = new NetworkVariable<float>(4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> projectileCooldown = new NetworkVariable<float>(1.4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> knockbackBuildUp = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> blockCooldown = new NetworkVariable<float>(3.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> maxProjectileSpeed = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> minProjectileSpeed = new NetworkVariable<float>(50f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);   
    public NetworkVariable<float> projectileChargeTime = new NetworkVariable<float>(1.2f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);     
    // public float startingHealth = 20f;

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         playerHealth.Value = startingHealth;
    //     }
    // }


    // Start is called before the first frame update
    // void Start()
    // {
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
