using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{

    //public NetworkVariable<int> health = new NetworkVariable<int>(20, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Slider healthBarSlider;

    public GameObject FillSliderHolder;

    public Slider visibleHealthBarSlider;

    public Canvas visibleHealthBarCanvas;
    
    public Renderer playerRenderer;

    public Renderer eyeCube;

    public PlayerStatsManager statsManager;

    public GameObject knockbackPercentObject;

    private void Start()
    {   
        if (IsLocalPlayer)
        {
            playerRenderer.enabled = false;
            eyeCube.enabled = false;
            FillSliderHolder = GameObject.Find("FillSliderHolder");
            healthBarSlider = FillSliderHolder.GetComponent<Slider>();
            visibleHealthBarCanvas.enabled = false;
            knockbackPercentObject = GameObject.Find("KnockbackPercent");
            knockbackPercentObject.GetComponent<TextMeshProUGUI>().text = statsManager.knockbackBuildUp.Value.ToString();
        }
        statsManager.knockbackBuildUp.OnValueChanged += OnKnockbackChanged;
        SetKnockback(statsManager.knockbackBuildUp.Value);
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

    private void SetKnockback(float value){
        if (IsLocalPlayer){
            knockbackPercentObject.GetComponent<TextMeshProUGUI>().text = value.ToString();
        }
    }

    private void OnHealthChanged(float oldValue, float newValue){
        SetHealth(newValue);
    }

    private void OnKnockbackChanged(float oldValue, float newValue){
        SetKnockback(newValue);
    }

}
