using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundEffectPlayer : NetworkBehaviour
{

    public AudioSource src;

    public AudioClip directHitSound;
    public AudioClip indirectHitSound;

    public AudioClip blastSound;
    // Start is called before the first frame update
    void Awake()
    {
        // if (src == null)
        // {
        //     Debug.LogError("AudioSource component not found on the GameObject.");
        // }
        // else
        // {
        //     Debug.Log("AudioSource component found.");
        // }
    }

    // // Update is called once per frame

    public void OnDirectHit(ulong attackerId, ulong receiverId){
        //TODO: Make an individual sound for getting hit vs hitting
        //Hitting: Hit marker has constant audio Distinct, only heard by 1
        //Getting hit: Hit sound only heard by 1, distinct
        //Hearing hit: Heard by all others nearby, distinct, volume based on range
        ulong[] clientIds = new ulong[] { attackerId, receiverId };
        DirectHitRpc(RpcTarget.Group(clientIds, RpcTargetUse.Temp));
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void DirectHitRpc(RpcParams rpcParams)
    {
        if (src == null){
            Debug.Log("Darth Vader Nooooooooooo");
            return;
        }
        Debug.Log("recieving on this computer");
        src.clip = directHitSound;
        src.Play();
        if (IsServer){
            StartCoroutine(DestroyAfterSound(blastSound));
        }
    }

    public void OnIndirectHit(List<ulong> receiverIdsList, ulong attackerId){
        // foreach (ulong clientId in receiverIdsList){
        //     Debug.Log("id = " + clientId);
        // }
        InDirectHitReceiveRpc(RpcTarget.Group(receiverIdsList, RpcTargetUse.Temp));
        //inDirectHitOriginatorRpc(RpcTarget.Single(attackerId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InDirectHitReceiveRpc(RpcParams rpcParams)
    {
        src.clip = indirectHitSound;
        src.Play();
        if (IsServer){
            StartCoroutine(DestroyAfterSound(blastSound));
        }
    }

    private void InDirectHitOriginatorRpc(RpcParams rpcParams)
    {
        src.clip = indirectHitSound;
        src.Play();
    }

    public void PlayBlastSound()
    {
        PlayBlastSoundRpc();
    }


    [Rpc(SendTo.Everyone)]
    private void PlayBlastSoundRpc(){
        src.clip = blastSound;
        src.volume = 1f;
        src.spatialBlend = 1.0f;
        src.minDistance = 1f;
        src.maxDistance = 2f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.Play();
        if (IsServer){
            StartCoroutine(DestroyAfterSound(blastSound));
        }
    }

    private IEnumerator DestroyAfterSound(AudioClip audio)
    {
        // Wait until the sound has finished playing
        yield return new WaitForSeconds(audio.length);
        NetworkObject.Despawn();
    }
}
