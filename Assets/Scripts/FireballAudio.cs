using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FireballAudio : NetworkBehaviour
{

    public GameObject audioSrcPrefab;

    public void PlayBlastSound()
    {
        GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
        audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);

        SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.PlayBlastSound();
        }
        else
        {
            Debug.LogError("SoundEffectPlayer component not found on audioSrcInstance.");
        }
    }

    public void PlayHitSound(ulong id1, ulong id2){
        GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
        audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);

        SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.OnDirectHit(id1, id2);
        }
        else
        {
            Debug.LogError("SoundEffectPlayer component not found on audioSrcInstance.");
        }
    }
}
