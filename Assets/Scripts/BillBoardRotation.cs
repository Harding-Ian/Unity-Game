using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BillBoardRotation : NetworkBehaviour
{
    public GameObject player;
    private ulong instanceId;
    private ulong playerToLookAtId;

    void LateUpdate()
    {
        if(IsLocalPlayer) return;

        playerToLookAtId = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerDeath>().playerSpectatingId.Value;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            instanceId = instance.GetComponent<PlayerScript>().clientId.Value;
            if (instanceId == playerToLookAtId)
            {
                transform.LookAt(instance.transform);
                return;
            }
        }
    }
}
