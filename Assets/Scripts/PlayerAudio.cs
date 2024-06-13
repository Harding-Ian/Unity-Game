using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
    private AudioListener audioListener;

    private void Awake()
    {
        audioListener = GetComponent<AudioListener>();

        // Ensure the audio listener is initially disabled
        if (audioListener != null)
        {
            audioListener.enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Enable the audio listener only if this is the local player
        if (IsOwner && audioListener != null)
        {
            audioListener.enabled = true;
        }
    }
}
