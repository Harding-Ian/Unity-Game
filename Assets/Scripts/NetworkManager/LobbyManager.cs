using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private GameObject uISceneManager;
    private GameObject MainMenuUI;
    private Lobby lobby;
    private float heartbeatTImer = 15f;
    private float lobbyUpdateTimer = 1.1f;
    private bool joinedGame = false;

    void Start()
    {
        uISceneManager = GameObject.Find("UISceneManager");
        MainMenuUI = GameObject.Find("MainMenuUI");
    }

    void OnClientConnected(ulong clientId)
    {
        if(NetworkManager.Singleton.ConnectedClientsList.Count != lobby.Players.Count) return;
        uISceneManager.GetComponent<UISceneManager>().CallPlayerScene();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }


    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 6;
            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            Debug.Log(lobby.LobbyCode);


            var data = new Dictionary<string, DataObject>
            {
                { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, "") }
            };

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { Data = data });

        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public void CheckLobby()
    {
        Debug.Log("player count: " + lobby.Players.Count);
        foreach (var player in lobby.Players) Debug.Log(player.Id);
        
    }


    public async void JoinLobbyByCode(string LobbyCode)
    {
        try
        {
            lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(LobbyCode);
            if(lobby != null) MainMenuUI.GetComponent<MainMenuScript>().LobbyUIToLobbyGameMenu();
        }
        catch (LobbyServiceException e) {Debug.Log(e);}
    }

    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch (LobbyServiceException e) {Debug.Log(e);}
    }

    public async void LeaveLobby()
    {
        await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);
        lobby = null;
    }

    public void Update()
    {
        SendLobbyHeartbeat();
        PingLobbyUpdate();
        if(Input.GetKeyDown(KeyCode.Q)) CheckLobby();
    }

    private async void SendLobbyHeartbeat()
    {
        if(lobby == null) return;
        if(lobby.HostId != AuthenticationService.Instance.PlayerId) return;
        

        heartbeatTImer -= Time.deltaTime;
        if(heartbeatTImer > 0) return;
        
        heartbeatTImer = 15f;
        await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
    }

    private async void PingLobbyUpdate()
    {
        if(lobby == null) return;
        lobbyUpdateTimer -= Time.deltaTime;
        if(lobbyUpdateTimer > 0) return;
        lobbyUpdateTimer = 1.1f;

        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);

        if(lobby.Data["joinCode"].Value != "" && joinedGame == false)
        {
            joinedGame = true;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobby.Data["joinCode"].Value);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
    }

    public async void StartGame()
    {
        if(lobby.HostId != AuthenticationService.Instance.PlayerId) return;

        joinedGame = true;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        //Debug.Log("join code: " + joinCode);

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        var data = new Dictionary<string, DataObject>
        {
            { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
        };

        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { Data = data });

    }
}
