using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDeath : NetworkBehaviour
{
    public GameObject playerCamera;

[Rpc(SendTo.SpecifiedInParams)]
    public void initiateDeathRpc(ulong killerId, RpcParams rpcParams)
    {
        Debug.Log("initiate death");
        playerCamera.GetComponent<Camera>().enabled = false;
        NetworkObject firstSpectatedPlayer = null;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().clientId.Value == killerId) firstSpectatedPlayer = instance.gameObject.GetComponent<NetworkObject>();
        }
        if(firstSpectatedPlayer == null) return;

        Debug.Log("firstSpectatedPlayer.Id ==== " + firstSpectatedPlayer.OwnerClientId);
        firstSpectatedPlayer.transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>().enabled = true;
        Debug.Log("firstSpectatedPlayer.transform.Find(CameraHolder).transform.Find(Camera) === " + firstSpectatedPlayer.transform.Find("CameraHolder").transform.Find("Camera"));
        
        firstSpectatedPlayer.transform.Find("Capsule").GetComponent<MeshRenderer>().enabled = false;
        //Debug.Log("firstSpectatedPlayer.cameraholder.activeself ==== " + firstSpectatedPlayer.GetComponent<PlayerDeath>().cameraHolder.activeSelf);
    }
}
