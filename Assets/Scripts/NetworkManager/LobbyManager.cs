using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    public GameObject uISceneManager;
    private async void Start() 
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += OnSignedIn;

        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void OnSignedIn()
    {
        Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 6;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            Debug.Log(lobby.LobbyCode);
        }
        catch (LobbyServiceException e) {Debug.Log(e);}
        uISceneManager.GetComponent<UISceneManager>().LoadGameMenu();
    }


    public async void JoinLobbyByCode(string LobbyCode)
    {
        try
        {
            await Lobbies.Instance.JoinLobbyByCodeAsync(LobbyCode);
        }
        catch (LobbyServiceException e) {Debug.Log(e);} 
    }
}
