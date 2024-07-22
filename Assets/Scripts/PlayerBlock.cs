using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBlock : NetworkBehaviour
{


    [Header("BlockButton")]
    public KeyCode blockKey = KeyCode.Mouse1;

    private bool readyToBlock = true;

    public PlayerStatsManager statsManager;

    public GameObject blockWave;
    
    void Update()
    {
        if (!IsLocalPlayer) return;
        if(Input.GetKeyDown(blockKey))
        {
            if (readyToBlock == true)
            {
                readyToBlock = false;
                block();
                Invoke(nameof(ResetBlock), statsManager.pulseCooldown.Value);
            }
        }
    }

    private void ResetBlock(){
        readyToBlock = true;
    }

    private void block(){
        BlockRpc(OwnerClientId, GetComponent<Transform>().position, GetComponent<PlayerStatsManager>().pulseRadius.Value);
    }

    [Rpc(SendTo.Server)]
    private void BlockRpc(ulong id, Vector3 blockOrigin, float radius){
        //GameObject blockObject = Instantiate(blockWave, GetComponent<Transform>().position, Quaternion.identity);
        SpawnBlockWaveRpc(blockOrigin);
        //blockObject.GetComponent<NetworkObject>().Spawn(true);

        GameObject gameManager = GameObject.Find("GameManager");

        Collider[] hitColliders = Physics.OverlapSphere(blockOrigin, radius);

        List<GameObject> playersInRange = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId != id)
            {
                if (!playersInRange.Contains(hitCollider.transform.root.gameObject))
                {
                    playersInRange.Add(hitCollider.transform.root.gameObject);
                }
            }
        }

        foreach (var player in playersInRange)
        {
            var ray = new Ray(GetComponent<Transform>().position, player.GetComponent<Transform>().position - GetComponent<Transform>().position);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit)) 
            {
                GameObject objectHit = hit.collider.transform.root.gameObject;
                if (objectHit.CompareTag("Player"))
                {
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, GetComponent<PlayerStatsManager>().pulseDamage.Value, id);
                    
                    player.GetComponent<PlayerKnockback>().ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, GetComponent<PlayerStatsManager>().pulseKnockbackForce.Value, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, GetComponent<PlayerStatsManager>().pulseKnockbackPercentDamage.Value);
                }
            }

        }
        
    }

    [Rpc(SendTo.Everyone)]
    private void SpawnBlockWaveRpc(Vector3 blockOrigin){
        GameObject blockObject = Instantiate(blockWave, blockOrigin, Quaternion.identity);
    }
    

}
