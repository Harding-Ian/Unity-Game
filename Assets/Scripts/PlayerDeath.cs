using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerDeath : NetworkBehaviour
{
    public ulong playerSpectatingId;


    public GameObject playerCamera;


    private void Start()
    {
        playerSpectatingId = OwnerClientId;
    }

    [Rpc(SendTo.Server)]
    public void ServerSideDeathRpc(ulong playerToDieId)
    {
        NetworkObject PlayerToDie = null;
        PlayerToDie = NetworkManager.Singleton.ConnectedClients[playerToDieId].PlayerObject;
        GetComponent<PlayerScript>().dead.Value = true;
        DisablePlayer(playerToDieId);

        ClientSideDeathRpc(GetComponent<PlayerScript>().lastDamagingPlayerId.Value, RpcTarget.Single(playerToDieId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ClientSideDeathRpc(ulong killerId, RpcParams RpcParams)
    {
        ChangeSpectator(killerId);
    }



    public void ChangeSpectator(ulong playerToSpectateId)
    {
        NetworkObject playerToSpectate = null;
        NetworkObject playerSpectating = null;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().clientId.Value == playerToSpectateId) playerToSpectate = instance.gameObject.GetComponent<NetworkObject>();
            if (instance.GetComponent<PlayerScript>().clientId.Value == playerSpectatingId) playerSpectating = instance.gameObject.GetComponent<NetworkObject>();
        }

        playerSpectating.transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>().enabled = false;
        playerToSpectate.transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>().enabled = true;

        SetVisibility(playerSpectating, true);
        SetVisibility(playerToSpectate, false);

        playerSpectatingId = playerToSpectateId;
        Debug.Log("set playerSpectatingId to " + playerSpectatingId);

    }


    public void SetVisibility(NetworkObject PlayerToChangeVisibility, bool visibility)
    {
        Debug.Log("setting visibility of " + PlayerToChangeVisibility + " to " + visibility);
        PlayerToChangeVisibility.transform.Find("Capsule").GetComponent<MeshRenderer>().enabled = visibility;
        PlayerToChangeVisibility.transform.Find("Visor").GetComponent<MeshRenderer>().enabled = visibility;
        PlayerToChangeVisibility.transform.Find("VisibleHealthBarCanvas").GetComponent<Canvas>().enabled = visibility;
    }

    public void DisablePlayer(ulong playerToDisableId)
    {
        NetworkObject playerToDisable = NetworkManager.Singleton.ConnectedClients[playerToDisableId].PlayerObject;

        playerToDisable.GetComponent<PlayerMovement>().enabled = false;
        playerToDisable.GetComponent<MouseLook>().enabled = false;
        playerToDisable.GetComponent<Projectile>().enabled = false;
        playerToDisable.GetComponent<PlayerBlock>().enabled = false;

    }


    void Update()
    {
        if(!IsLocalPlayer) return;

        if(Input.GetKey(KeyCode.Q))
        {
            Debug.Log("last damaging player client id ==== " + GetComponent<PlayerScript>().lastDamagingPlayerId.Value);
        }
        
        if(Input.GetKey(KeyCode.E))
        {
            Debug.Log("playerspectatingId ==== " + GetComponent<PlayerDeath>().playerSpectatingId);
        }
    }
    

}
