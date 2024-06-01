using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{

    public NetworkVariable<int> health = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Slider healthBarSlider;

    public GameObject HealthBarUI;


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
            health.OnValueChanged += OnHealthChanged;
            HealthBarUI = GameObject.Find("HealthBarUI");
            healthBarSlider = HealthBarUI.GetComponent<Slider>();
            if (health != null){
                SetMaxHealth(health.Value);
            }
            else{
                Debug.Log("Bababooey");
            }
        }
    }



    public void SetMaxHealth(int health){
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }

    public void SetHealth(int health){
        healthBarSlider.value = health;
        Debug.Log("updated slider health to " + health);
    }

    public void logHealth(ulong id){
        Debug.Log("health of player " + id + "= " + health.Value);
    }

    private void OnHealthChanged(int oldValue, int newValue){
        SetHealth(newValue);
    }
}
