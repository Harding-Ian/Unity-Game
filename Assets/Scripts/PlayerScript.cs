using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    [NonSerialized] public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>(100000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [NonSerialized] public NetworkVariable<ulong> lastDamagingPlayerId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [NonSerialized] public NetworkVariable<bool> dead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [NonSerialized] public NetworkVariable<bool> upgraded = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [NonSerialized] public NetworkVariable<int> wins = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [NonSerialized] public List<string> UpgradeList = new List<string>();

    private float FOV = 85f;
    private Camera Camera;

    private void Start()
    {
        if (IsServer)
        {
            clientId.Value = OwnerClientId;
            lastDamagingPlayerId.Value = NetworkManager.ServerClientId;
            if (clientId.Value == lastDamagingPlayerId.Value) lastDamagingPlayerId.Value += 1;

            Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            Color randomColor2 = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            
            randomColourRpc(randomColor, randomColor2);
        }
        if(IsLocalPlayer)
        {
            Camera = transform.Find("CameraHolder/Camera").GetComponent<Camera>();
        }
    }

    void Update()
    {
        Camera.fieldOfView = PlayerPrefs.GetFloat("FOV");
    }

    [Rpc(SendTo.Everyone)]
    public void randomColourRpc(Color randomColour, Color randomColour2){

        
        Renderer renderer = transform.Find("Model/Body").GetComponent<Renderer>();
        Renderer renderer2 = transform.Find("Model/Hat").GetComponent<Renderer>();
        
        renderer.materials[0].color = randomColour;
        renderer.materials[2].color = randomColour2;

        renderer2.materials[0].color = randomColour;
    }
}
