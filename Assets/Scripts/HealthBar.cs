using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{

    public NetworkVariable<int> health = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

   
    public Slider slider;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 20;
        }
    }

    public void SetMaxHealth(int health){
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health){
        slider.value = health;
    }

    public void logHealth(ulong id){
        Debug.Log("health of player " + id + "= " + health.Value);
    }
}
