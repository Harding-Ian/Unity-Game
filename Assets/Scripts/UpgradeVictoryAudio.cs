using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UpgradeVictoryAudio : NetworkBehaviour
{

    public AudioSource src;
    public AudioClip victoryScreech;
    

    private void OnTriggerEnter(Collider collider){
        //PlayVictoryScreechRpc(RpcTarget.Single(collider.transform.root.GetComponent<PlayerScript>().clientId.Value, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void PlayVictoryScreechRpc(RpcParams rpcParams){
        src.clip = victoryScreech;
        src.Play();
    }

}
