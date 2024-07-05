using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBorder : NetworkBehaviour
{


    private void OnTriggerEnter(Collider collider)
    {
        if (IsServer && collider.gameObject.CompareTag("Player") && collider.gameObject.GetComponent<PlayerScript>().dead.Value == false) 
        {
            Debug.Log("worldborder killing player " + collider.gameObject.GetComponent<PlayerScript>().clientId.Value);
            collider.gameObject.GetComponent<PlayerDeath>().InitiatePlayerDeath();
        }
    }

}




