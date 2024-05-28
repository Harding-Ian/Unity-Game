using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

public class HealthManager : NetworkBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
        {
            healthBar = FindObjectOfType<HealthBar>();
        }


        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)){
            currentHealth = maxHealth;
            healthBar.SetHealth(currentHealth);
        }
    }

    public void reduceHealth(int damageAmount){
        currentHealth -= damageAmount;
        healthBar.SetHealth(currentHealth);
    }


    [ServerRpc]
    public void ReduceHealthServerRpc(int damageAmount){
        currentHealth -= damageAmount;
        healthBar.SetHealth(currentHealth);
    }

    [ClientRpc]
    public void ReduceHealthClientRpc(int damageAmount){
        currentHealth -= damageAmount;
        healthBar.SetHealth(currentHealth);
    }

}
