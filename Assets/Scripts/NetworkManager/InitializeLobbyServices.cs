using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class InitializeLobbyServices : MonoBehaviour
{
    private async void Start() 
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += OnSignedIn;

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Old = " + AuthenticationService.Instance.PlayerId);
            AuthenticationService.Instance.ClearSessionToken();
            AuthenticationService.Instance.
            string profilename = Random.Range(0, 100000).ToString();
            Debug.Log(profilename);
            AuthenticationService.Instance.SwitchProfile(profilename);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void OnSignedIn()
    {
        Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) Debug.Log(AuthenticationService.Instance.PlayerId);
    }

}
