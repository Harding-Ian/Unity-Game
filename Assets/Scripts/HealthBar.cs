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

    public Slider visibleHealthBarSlider;

    public Canvas visibleHealthBarCanvas;
    
    public Renderer playerRenderer;

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
            playerRenderer.enabled = false;
            HealthBarUI = GameObject.Find("HealthBarUI");
            healthBarSlider = HealthBarUI.GetComponent<Slider>();
            visibleHealthBarCanvas.enabled = false;
        }
        if (health != null){
                SetMaxHealth(health.Value);
            }
        else{
            Debug.Log("Error: Health == null");
        }
        health.OnValueChanged += OnHealthChanged;
    }


    public void SetMaxHealth(int health){
        if (IsLocalPlayer){
            healthBarSlider.maxValue = health;
            healthBarSlider.value = health;
        }
        visibleHealthBarSlider.maxValue = health;
        visibleHealthBarSlider.value = health;
    }

    public void SetHealth(int health){
        if (IsLocalPlayer){
            healthBarSlider.value = health;
        }
        visibleHealthBarSlider.value = health;
    }

    private void OnHealthChanged(int oldValue, int newValue){
        SetHealth(newValue);
    }


}
