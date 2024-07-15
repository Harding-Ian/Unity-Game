using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerDeath : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<ulong> playerSpectatingId = new NetworkVariable<ulong>(1000003, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameObject playerCamera;
    public GameObject GameManager;


    private void Start()
    {
        GameManager = GameObject.Find("GameManager");
        if(IsLocalPlayer)
        {
            ChangeplayerSpectatingIdRpc(OwnerClientId);
            playerSpectatingId.OnValueChanged += playerSpectatingIdObserver;
        }
    }


    public void playerSpectatingIdObserver(ulong oldValue, ulong newValue)
    {
        if(oldValue == 1000003) return;
        ChangeSpectator(newValue, oldValue);
    }


    public void InitiatePlayerDeath()
    {
        ulong playerToDieId = OwnerClientId;
        GetComponent<PlayerScript>().dead.Value = true;
        DisablePlayerRpc(RpcTarget.Single(playerToDieId, RpcTargetUse.Temp));

        List<PlayerScript> AlivePlayersList = new List<PlayerScript>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerDeath>().playerSpectatingId.Value == playerToDieId)
            {
                instance.GetComponent<PlayerDeath>().playerSpectatingId.Value = NetworkManager.Singleton.ConnectedClients[playerToDieId].PlayerObject.GetComponent<PlayerScript>().lastDamagingPlayerId.Value;
            }

            if (instance.GetComponent<PlayerScript>().dead.Value == false)
            {
                AlivePlayersList.Add(instance);
            }
        }

        if(AlivePlayersList.Count < 2)
        {
            GameManager.GetComponent<GameSceneManager>().RoundCompleted(AlivePlayersList[0].GameObject());
        }
        
    }


    public ulong FindPlayerToSpectateId(int dir)
    {
        List<ulong> AlivePlayerIds = new List<ulong>();
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().dead.Value == false) AlivePlayerIds.Add(instance.GetComponent<PlayerScript>().clientId.Value);
        }

        AlivePlayerIds.Sort();
        ulong lastplayerId = AlivePlayerIds[AlivePlayerIds.Count - 1];
        ulong firstplayerId = AlivePlayerIds[0];

        if (playerSpectatingId.Value == firstplayerId && dir < 0) return lastplayerId;
        else if (playerSpectatingId.Value == lastplayerId && dir > 0) return firstplayerId;
        else return AlivePlayerIds[AlivePlayerIds.IndexOf(playerSpectatingId.Value) + dir];
    }


    public void ChangeSpectator(ulong playerToSpectateId, ulong playerWasSpectatingId)
    {
        if(playerToSpectateId == playerWasSpectatingId) return;

        NetworkObject playerToSpectate = null;
        NetworkObject playerWasSpectating = null;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if (instance.GetComponent<PlayerScript>().clientId.Value == playerToSpectateId) playerToSpectate = instance.gameObject.GetComponent<NetworkObject>();
            if (instance.GetComponent<PlayerScript>().clientId.Value == playerWasSpectatingId) playerWasSpectating = instance.gameObject.GetComponent<NetworkObject>();
        }

        playerWasSpectating.GetComponent<PlayerStatsManager>().playerHealth.OnValueChanged -= GetComponent<HealthBar>().OnHealthChanged;
        playerToSpectate.GetComponent<PlayerStatsManager>().playerHealth.OnValueChanged += GetComponent<HealthBar>().OnHealthChanged;
        GetComponent<HealthBar>().OnHealthChanged(playerWasSpectating.GetComponent<PlayerStatsManager>().playerHealth.Value, playerToSpectate.GetComponent<PlayerStatsManager>().playerHealth.Value);

        playerWasSpectating.GetComponent<PlayerStatsManager>().knockbackBuildUp.OnValueChanged -= GetComponent<HealthBar>().OnKnockbackChanged;
        playerToSpectate.GetComponent<PlayerStatsManager>().knockbackBuildUp.OnValueChanged += GetComponent<HealthBar>().OnKnockbackChanged;
        GetComponent<HealthBar>().OnKnockbackChanged(playerWasSpectating.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value, playerToSpectate.GetComponent<PlayerStatsManager>().knockbackBuildUp.Value);

        playerWasSpectating.transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>().enabled = false;
        playerToSpectate.transform.Find("CameraHolder").transform.Find("Camera").GetComponent<Camera>().enabled = true;

        SetVisibility(playerWasSpectating, true);
        SetVisibility(playerToSpectate, false);
    }


    public void SetVisibility(NetworkObject PlayerToChangeVisibility, bool visibility)
    {
        //PlayerToChangeVisibility.transform.Find("Capsule").GetComponent<MeshRenderer>().enabled = visibility;
        //PlayerToChangeVisibility.transform.Find("Visor").GetComponent<MeshRenderer>().enabled = visibility;

        PlayerToChangeVisibility.transform.Find("Model/Head").GetComponent<MeshRenderer>().enabled = visibility;
        PlayerToChangeVisibility.transform.Find("Model/Body").GetComponent<MeshRenderer>().enabled = visibility;
        PlayerToChangeVisibility.transform.Find("Model/Eyes").GetComponent<MeshRenderer>().enabled = visibility;
        PlayerToChangeVisibility.transform.Find("Model/Hat").GetComponent<MeshRenderer>().enabled = visibility;

        PlayerToChangeVisibility.transform.Find("VisibleHealthBarCanvas").GetComponent<Canvas>().enabled = visibility;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void DisablePlayerRpc(RpcParams rpcParams)
    {
        NetworkObject playerToDisable = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        playerToDisable.GetComponent<PlayerMovement>().enabled = false;
        playerToDisable.GetComponent<MouseLook>().enabled = false;
        playerToDisable.GetComponent<Projectile>().enabled = false;
        playerToDisable.GetComponent<PlayerBlock>().enabled = false;

    }


    void Update()
    {
        if(!IsLocalPlayer) return;

        if(GetComponent<PlayerScript>().dead.Value == false) return;
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ChangeplayerSpectatingIdRpc(FindPlayerToSpectateId(-1));
        }
        
        if(Input.GetKeyDown(KeyCode.E))
        {
            ChangeplayerSpectatingIdRpc(FindPlayerToSpectateId(1));
        }
    }


    [Rpc(SendTo.Server)]
    public void ChangeplayerSpectatingIdRpc(ulong newId)
    {
        playerSpectatingId.Value = newId;
    }

}
