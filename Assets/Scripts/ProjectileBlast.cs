using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBlast : NetworkBehaviour
{


    private ulong playerOwnerId;
    public string playerTag = "Player";

    public GameObject audioSrcPrefab;

    public GameObject gameManager;

    void Start()
    {
        if (IsServer){
            gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found within scene");
            }

            Invoke("DestroyProjectile", 0.3f);
            FindPlayers();
        }
        
    }

    public void SetPlayerWhoFired(ulong playerId){
        playerOwnerId = playerId;
    }


    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }

    private void FindPlayers(){
        Vector3 blastCenter = transform.position;

        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        Collider[] hitColliders = Physics.OverlapSphere(blastCenter, playerWhoShot.explosionRadius.Value);

        List<GameObject> playersInRange = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(playerTag))
            {
                playersInRange.Add(hitCollider.gameObject);
            }
        }

        int i = 0;
        List<ulong> clientIdsList = new List<ulong>();
        
        foreach (var player in playersInRange) {
            i += 1;

            var ray = new Ray(GetComponent<Transform>().position, player.GetComponent<Transform>().position - GetComponent<Transform>().position);
            RaycastHit hit;
            
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider.bounds.Contains(ray.origin)) {
                gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionDamage.Value, playerOwnerId);
                ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionKnockbackPercentDamage.Value);
            }
            else if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.collider.gameObject;
                if (objectHit.CompareTag("Player")){
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionDamage.Value, playerOwnerId);
                    ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionKnockbackPercentDamage.Value);

                    if (playerOwnerId != player.GetComponent<NetworkObject>().OwnerClientId){
                        clientIdsList.Add(player.GetComponent<NetworkObject>().OwnerClientId);

                    }
                }
            }
        }
        if (clientIdsList.Count > 0){
            GameObject audioSrcInstance = Instantiate(audioSrcPrefab, transform.position, Quaternion.identity);
            audioSrcInstance.GetComponent<NetworkObject>().Spawn(true);
        
            SoundEffectPlayer soundPlayer = audioSrcInstance.GetComponent<SoundEffectPlayer>();
            if (soundPlayer != null)
            {
                soundPlayer.OnIndirectHit(clientIdsList, playerOwnerId);
            }
            else
            {
                Debug.LogError("SoundEffectPlayer component not found on audioSrcInstance.");
            }
        }
    }



    [Rpc(SendTo.SpecifiedInParams)]
    private void ApplyKnockbackRpc(Vector3 knockbackDirection, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();
        

        float angle = 180 - Vector3.Angle(knockbackDirection, Vector3.down) - 65;

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;

        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);
        playerNetworkObject.GetComponent<PlayerMovement>().ApplyKnockback(adjustedknockbackDirection, playerWhoShot.explosionKnockbackForce.Value);
    }
    

}
