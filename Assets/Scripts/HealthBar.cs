using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{

    //public NetworkVariable<int> health = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Slider healthBarSlider;

    public GameObject HealthBarUI;

    public Slider visibleHealthBarSlider;

    public Canvas visibleHealthBarCanvas;
    
    public Renderer playerRenderer;

    public Renderer eyeCube;

    public PlayerStatsManager statsManager;

    private void Start()
    {   
        if (IsLocalPlayer)
        {
            playerRenderer.enabled = false;
            eyeCube.enabled = false;
            HealthBarUI = GameObject.Find("HealthBarUI");
            healthBarSlider = HealthBarUI.GetComponent<Slider>();
            visibleHealthBarCanvas.enabled = false;
        }

        SetMaxHealth(statsManager.playerHealth.Value);
        statsManager.playerHealth.OnValueChanged += OnHealthChanged;
    }


    public void SetMaxHealth(float health){
        if (IsLocalPlayer){
            healthBarSlider.maxValue = health;
            healthBarSlider.value = health;
        }
        visibleHealthBarSlider.maxValue = health;
        visibleHealthBarSlider.value = health;
    }

    public void SetHealth(float health){
        if (IsLocalPlayer){
            healthBarSlider.value = health;
        }
        visibleHealthBarSlider.value = health;
    }

    private void OnHealthChanged(float oldValue, float newValue){
        SetHealth(newValue);
    }


}
