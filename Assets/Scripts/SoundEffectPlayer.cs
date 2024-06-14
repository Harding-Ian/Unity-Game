using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundEffectPlayer : NetworkBehaviour
{

    public AudioSource src;

    public AudioClip directHitSound;
    public AudioClip indirectHitSound;
    // Start is called before the first frame update
    void Start()
    {
        GameObject audioSourceObject = GameObject.Find("AudioSrc");
        if (audioSourceObject != null)
        {
            src = audioSourceObject.GetComponent<AudioSource>();
            if (src == null)
            {
                Debug.LogError("AudioSource component not found on AudioSrc GameObject.");
            }
            else{
                Debug.Log("Src found");
            }
        }
        else
        {
            Debug.LogError("AudioSrc GameObject not found in the scene.");
        }
    }

    // // Update is called once per frame

    public void onDirectHit(ulong attackerId, ulong receiverId){
        //TODO: Make an individual sound for getting hit vs hitting
        //Hitting: Hit marker has constant audio Distinct, only heard by 1
        //Getting hit: Hit sound only heard by 1, distinct
        //Hearing hit: Heard by all others nearby, distinct, volume based on range
        ulong[] clientIds = new ulong[] { attackerId, receiverId };
        directHitRpc(RpcTarget.Group(clientIds, RpcTargetUse.Temp));
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void directHitRpc(RpcParams rpcParams)
    {
        Debug.Log("recieving on this computer");
        src.clip = directHitSound;
        src.Play();
    }

    public void onIndirectHit(ulong attackerId, ulong receiverId){
        ulong[] clientIds = new ulong[] { attackerId, receiverId };
        directHitRpc(RpcTarget.Group(clientIds, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void inDirectHitRpc(RpcParams rpcParams)
    {
        Debug.Log("recieving on this computer");
        src.clip = indirectHitSound;
        src.Play();
    }
}
