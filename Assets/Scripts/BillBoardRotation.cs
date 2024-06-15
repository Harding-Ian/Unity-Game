using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BillBoardRotation : NetworkBehaviour
{
    public GameObject player;
    private ulong instanceID;

    void LateUpdate()
    {
        if(!IsLocalPlayer) {
            foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
            {
                if (instance.IsLocalPlayer)
                {
                    //instanceID = instance.GetComponent<PlayerDeath>().playerSpectatingId;
                    transform.LookAt(instance.transform);
                    return;
                }
            }
            
            // foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
            // {
            //     if (instance.GetComponent<PlayerScript>().clientId.Value == instanceID) 
            //     {
            //         transform.LookAt(instance.transform);
            //     }
            // }
            
        }
    }
}
