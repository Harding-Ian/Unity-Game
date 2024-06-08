using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour
{

    public NetworkVariable<float> playerHealth = new NetworkVariable<float>(20f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> projectileCooldown = new NetworkVariable<float>(1.4f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> knockbackBuildUp = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    // public float startingHealth = 20f;

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         playerHealth.Value = startingHealth;
    //     }
    // }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HEALTH ===== " + playerHealth.Value);
        knockbackBuildUp.Value = 1f;
        Debug.Log("KB ===== " + knockbackBuildUp.Value);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
