using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerDeath : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<ulong> playerSpectatingId = new NetworkVariable<ulong>(1000003, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameObject playerCamera;
    public GameObject GameManager;

    public GameObject ScreenUI;
    TextMeshProUGUI SpectatingText;

    public ParticleSystem deathParticles;

    public GameObject hatPrefab;


    private void Start()
    {
        GameManager = GameObject.Find("GameManager");
        ScreenUI = GameObject.Find("ScreenUI");
        SpectatingText = ScreenUI.transform.Find("Spectating").GetComponent<TextMeshProUGUI>();

        if(IsLocalPlayer)
        {
            ChangeplayerSpectatingIdRpc(OwnerClientId);
            playerSpectatingId.OnValueChanged += playerSpectatingIdObserver;
            GetComponent<PlayerScript>().dead.OnValueChanged += onDeadChanged;
        }
    }


    public void playerSpectatingIdObserver(ulong oldValue, ulong newValue)
    {
        if(oldValue == 1000003) return;
        ChangeSpectator(newValue, oldValue);
    }

    public void onDeadChanged(bool oldValue, bool newValue)
    {
        if(newValue == oldValue) return;
        if(newValue == true) ScreenUI.transform.Find("Spectating").gameObject.SetActive(true);
        if(newValue == false) ScreenUI.transform.Find("Spectating").gameObject.SetActive(false);
    }


    public void InitiatePlayerDeath()
    {

        playDeathParticlesRpc();
        spawnHatRpc(transform.position);

        ulong playerToDieId = OwnerClientId;
        GetComponent<PlayerScript>().dead.Value = true;
        DisablePlayerRpc(RpcTarget.Single(playerToDieId, RpcTargetUse.Temp));

        
        List<PlayerScript> AlivePlayersList = new List<PlayerScript>();

        ulong playerToSpectateId = NetworkManager.Singleton.ConnectedClients[playerToDieId].PlayerObject.GetComponent<PlayerScript>().lastDamagingPlayerId.Value;

        if(NetworkManager.Singleton.ConnectedClients[playerToSpectateId].PlayerObject.GetComponent<PlayerScript>().dead.Value)
        {
            foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
            {
                if (instance.GetComponent<PlayerScript>().dead.Value == false)
                {
                    playerToSpectateId = instance.GetComponent<NetworkObject>().OwnerClientId;
                    break;
                }
            }
        }
        
        if(NetworkManager.Singleton.ConnectedClients[playerToSpectateId].PlayerObject.GetComponent<PlayerScript>().dead.Value)
        {
            playerToSpectateId = playerToDieId;
        }

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            
            if (instance.GetComponent<PlayerDeath>().playerSpectatingId.Value == playerToDieId)
            {
                instance.GetComponent<PlayerDeath>().playerSpectatingId.Value = playerToSpectateId;
            }

            if (instance.GetComponent<PlayerScript>().dead.Value == false)
            {
                AlivePlayersList.Add(instance);
            }
        }

        if(AlivePlayersList.Count == 1)
        {
            GameManager.GetComponent<GameSceneManager>().RoundCompleted(AlivePlayersList[0].GameObject());
        }
    }

    [Rpc(SendTo.Everyone)]
    private void playDeathParticlesRpc(){
        deathParticles.Play();
    }

    [Rpc(SendTo.Server)]
    private void spawnHatRpc(Vector3 pos){


        GameObject hat = Instantiate(hatPrefab, pos, Quaternion.identity);

        GameObject anchor = GameObject.Find("ObjectAnchor");

        hat.transform.parent = anchor.transform;

        hat.GetComponent<NetworkObject>().Spawn(true);
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

        SpectatingText.text = "Spectating: " + playerToSpectate.OwnerClientId.ToString();

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

        playerToDisable.GetComponent<PlayerInput>().enabled = false;
        playerToDisable.GetComponent<MouseLook>().enabled = false;
        playerToDisable.GetComponent<Projectile>().enabled = false;
        playerToDisable.GetComponent<PlayerBlock>().enabled = false;

        playerToDisable.GetComponent<Projectile>().resetSlidersRpc();

    }


    void Update()
    {
        if(!IsLocalPlayer) return;

        if(GetComponent<PlayerScript>().dead.Value == false) return;
        
        if(Input.GetKeyDown(KeyCode.Q) && GameManager.GetComponent<GameSceneManager>().spectatingBool)
        {
            ChangeplayerSpectatingIdRpc(FindPlayerToSpectateId(-1));
        }
        
        if(Input.GetKeyDown(KeyCode.E) && GameManager.GetComponent<GameSceneManager>().spectatingBool)
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
