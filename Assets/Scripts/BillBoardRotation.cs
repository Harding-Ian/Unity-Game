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
        if(!IsLocalPlayer) {
            foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
            {
                if (instance.IsLocalPlayer)
                {
                    transform.LookAt(instance.transform);
                    return;
                }
            }
        }
    }
}
