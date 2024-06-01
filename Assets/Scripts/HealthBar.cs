using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{

    public NetworkVariable<int> health = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Slider healthBarSlider;
    public GameObject playerUIPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 20;
        }
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            SpawnPlayerUI();
        }
    }

    private void SpawnPlayerUI()
    {
        GameObject playerUI = Instantiate(playerUIPrefab);
        healthBarSlider = playerUI.GetComponentInChildren<Slider>();

        // If you need to link the UI to the player, do it here
        // Example: playerUI.GetComponent<PlayerUI>().SetPlayer(this);
    }

    public void SetMaxHealth(int health){
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }

    public void SetHealth(int health){
        healthBarSlider.value = health;
        Debug.Log("updated slider health to " + health);
    }

    public void UpdateHealth(){
        //SetHealth(health.Value);
    }

    public void logHealth(ulong id){
        Debug.Log("health of player " + id + "= " + health.Value);
    }
}
