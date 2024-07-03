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
    
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;
        if(Input.GetKeyDown(blockKey))
        {
            if (readyToBlock == true)
            {
                readyToBlock = false;
                block();
                Invoke(nameof(ResetBlock), statsManager.blockCooldown.Value);
            }
        }
    }

    private void ResetBlock(){
        readyToBlock = true;
    }

    private void block(){
        BlockRpc(OwnerClientId, GetComponent<Transform>().position, 4f);
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
            if (hitCollider.CompareTag("Player"))
            {
                playersInRange.Add(hitCollider.gameObject);
            }
        }

        int i = 0;
        foreach (var player in playersInRange) {
            i += 1;

            if (player.GetComponent<NetworkObject>().OwnerClientId == id) {continue;}

            var ray = new Ray(GetComponent<Transform>().position, player.GetComponent<Transform>().position - GetComponent<Transform>().position);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit)) {
                GameObject objectHit = hit.collider.gameObject;
                if (objectHit.CompareTag("Player")){
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, 0.5f, id);
                    ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, 0.1f);
                }
            }

        }
        
    }

    [Rpc(SendTo.Everyone)]
    private void SpawnBlockWaveRpc(Vector3 blockOrigin){
        GameObject blockObject = Instantiate(blockWave, blockOrigin, Quaternion.identity);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ApplyKnockbackRpc(Vector3 knockbackDirection, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        

        float angle = 180 - Vector3.Angle(knockbackDirection, Vector3.down) - 65;

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;

        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(adjustedknockbackDirection, 12);
    }

}
