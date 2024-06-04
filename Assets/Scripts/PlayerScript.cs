using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>(100000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start(){
        if (IsServer){
            clientId.Value = OwnerClientId;
        }
    }
}
