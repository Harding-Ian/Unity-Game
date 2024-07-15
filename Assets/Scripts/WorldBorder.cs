using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WorldBorder : NetworkBehaviour
{


    private void OnTriggerEnter(Collider collider)
    {
        if (IsServer && collider.transform.root.CompareTag("Player") && collider.transform.root.GetComponent<PlayerScript>().dead.Value == false)
        {
            collider.transform.root.GetComponent<PlayerDeath>().InitiatePlayerDeath();
        }
    }

}




