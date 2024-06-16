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
        GetComponent<PlayerScript>().dead.Value = true;
        DisablePlayer(playerToDieId);

        List<ulong> PlayersToSwitchIds = new List<ulong>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerDeath>().playerSpectatingId == playerToDieId) PlayersToSwitchIds.Add(playerSpectatingId);
        }
        ClientSideSwitchSpectatorRpc(GetComponent<PlayerScript>().lastDamagingPlayerId.Value, RpcTarget.Group(PlayersToSwitchIds, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ClientSideSwitchSpectatorRpc(ulong killerId, RpcParams RpcParams)
    {
        ChangeSpectator(killerId);
    }

    public void SwitchSpectatorRight()
    {

        List<ulong> AlivePlayerIds = new List<ulong>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().dead.Value == false) AlivePlayerIds.Add(instance.GetComponent<PlayerScript>().clientId.Value);
        }
        if(AlivePlayerIds.Count == 0) return;

        AlivePlayerIds.Sort();
        if (playerSpectatingId == AlivePlayerIds[AlivePlayerIds.Count - 1])
        {
            ChangeSpectator(AlivePlayerIds[0]);
            ChangeplayerSpectatingIdRpc(AlivePlayerIds[0]);
            playerSpectatingId = AlivePlayerIds[0];
        }
        else 
        {
            ChangeSpectator(AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) + 1]);
            ChangeplayerSpectatingIdRpc(AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) + 1]);
            playerSpectatingId = AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) + 1];
        }
    }



    public void SwitchSpectatorLeft()
    {

        List<ulong> AlivePlayerIds = new List<ulong>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().dead.Value == false) AlivePlayerIds.Add(instance.GetComponent<PlayerScript>().clientId.Value);
        }
        if(AlivePlayerIds.Count == 0) return;

        AlivePlayerIds.Sort();
        if (playerSpectatingId == AlivePlayerIds[0]) 
        {
            ChangeSpectator(AlivePlayerIds[AlivePlayerIds.Count - 1]);
            ChangeplayerSpectatingIdRpc(AlivePlayerIds[AlivePlayerIds.Count - 1]);
            playerSpectatingId = AlivePlayerIds[AlivePlayerIds.Count - 1];
        }
        else 
        {
            ChangeSpectator(AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) - 1]);
            ChangeplayerSpectatingIdRpc(AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) - 1]);
            playerSpectatingId = AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId) - 1];
        }
        
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

        if(Input.GetKeyDown(KeyCode.Q))
        {
            SwitchSpectatorLeft();
            //Debug.Log("last damaging player client id ==== " + GetComponent<PlayerScript>().lastDamagingPlayerId.Value);
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            SwitchSpectatorRight();
            //Debug.Log("playerspectatingId ==== " + GetComponent<PlayerDeath>().playerSpectatingId);
        }
    }

    [Rpc(SendTo.Server)]
    public void ChangeplayerSpectatingIdRpc(ulong newId)
    {
        playerSpectatingId = newId;
        Debug.Log("playerSpectatingId set to " + newId);
    }

}
