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
            AuthenticationService.Instance.ClearSessionToken();

            string profilename = Random.Range(0, 100000).ToString();
            AuthenticationService.Instance.SwitchProfile(profilename);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    void OnSignedIn()
    {
        Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
    }

}
