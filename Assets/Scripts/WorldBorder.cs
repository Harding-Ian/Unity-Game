using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBorder : NetworkBehaviour
{


    private void OnTriggerEnter(Collider collider)
    {
        if (IsServer && collider.gameObject.CompareTag("Player")) collider.gameObject.GetComponent<PlayerDeath>().InitiatePlayerDeath();
    }


    
}




