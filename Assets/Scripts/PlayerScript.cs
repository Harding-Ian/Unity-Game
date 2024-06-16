using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>(100000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> lastDamagingPlayerId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> dead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        if (IsServer)
        {
            clientId.Value = OwnerClientId;
            lastDamagingPlayerId.Value = NetworkManager.ServerClientId;
            if (clientId.Value == lastDamagingPlayerId.Value) lastDamagingPlayerId.Value += 1;

            
        }
        Renderer renderer = GetComponentInChildren<Renderer>();

        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

        if (renderer != null)
        {
            renderer.material.color = randomColor;
        }
    }
}
