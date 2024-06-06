using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatsManager : NetworkBehaviour
{

    public NetworkVariable<float> playerHealth = new NetworkVariable<float>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
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
