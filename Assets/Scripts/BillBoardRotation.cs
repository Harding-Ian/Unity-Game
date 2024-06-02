using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BillBoardRotation : NetworkBehaviour
{
    public GameObject player;

    void LateUpdate()
    {
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance != player.GetComponent<PlayerScript>())
            {
                transform.rotation = instance.transform.rotation;
            }

            //if(!IsLocalPlayer) transform.rotation = quaternion.identity;
        }
    }
}
