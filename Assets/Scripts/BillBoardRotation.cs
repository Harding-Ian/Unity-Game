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
    [SerializeField]
    private bool card = false;

    void LateUpdate()
    {
        if(IsLocalPlayer) return;

        playerToLookAtId = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerDeath>().playerSpectatingId.Value;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            instanceId = instance.GetComponent<PlayerScript>().clientId.Value;
            if (instanceId == playerToLookAtId)
            {
                if(card) 
                {
                    //transform.LookAt(instance.transform);
                    transform.rotation = instance.transform.rotation;
                }
                else 
                {
                    //transform.LookAt(instance.transform);
                    transform.rotation = instance.transform.Find("CameraHolder").rotation;
                }
                return;
            }
        }
    }
}
