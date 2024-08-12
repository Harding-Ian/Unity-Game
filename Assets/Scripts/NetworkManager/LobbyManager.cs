using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private GameObject uISceneManager;
    private Lobby lobby;


    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 6;
            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            Debug.Log(lobby.LobbyCode);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public void CheckLobby()
    {
        Debug.Log("amount of players in lobby is " + lobby.Players.Count);
        foreach (var player in lobby.Players) Debug.Log(player.ConnectionInfo + " " + player.AllocationId + " " + player.Id + " " + player.Profile);
    }


    public async void JoinLobbyByCode(string LobbyCode)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByCodeAsync(LobbyCode);
        }
        catch (LobbyServiceException e) {Debug.Log(e);} 
    }

    public async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch (LobbyServiceException e) {Debug.Log(e);} 
    }
}
