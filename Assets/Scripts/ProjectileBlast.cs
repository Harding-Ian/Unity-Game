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

            Invoke(nameof(DestroyBlastObject), 1f); // REPLACE 1 /////////////////////
            FindPlayers();
        }
        
    }

    public void SetPlayerWhoFired(ulong playerId){
        playerOwnerId = playerId;
    }


    private void DestroyBlastObject()
    {
        NetworkObject.Despawn();
    }

    private void FindPlayers()
    {

        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerWhoShot.explosionRadius.Value);

        List<GameObject> playersInRange = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root.CompareTag(playerTag) && !playersInRange.Contains(hitCollider.transform.root.gameObject)) playersInRange.Add(hitCollider.transform.root.gameObject);
        }

        List<ulong> clientIdsList = new List<ulong>();
        
        foreach (var player in playersInRange)
        {
            var ray = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;

            if (player.GetComponent<Collider>().bounds.Contains(ray.origin)) 
            {
                gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionDamage.Value, playerOwnerId);
                gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionKnockbackPercentDamage.Value);
                player.GetComponent<PlayerKnockback>().ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, playerWhoShot.explosionKnockbackForce.Value, false, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
            }

            else if (Physics.Raycast(ray, out hit)) 
            {
                if (hit.collider.transform.root.gameObject.CompareTag("Player"))
                {
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionDamage.Value, playerOwnerId);
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, playerWhoShot.explosionKnockbackPercentDamage.Value);
                    
                    player.GetComponent<PlayerKnockback>().ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, playerWhoShot.explosionKnockbackForce.Value, false, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));

                    if (playerOwnerId != player.GetComponent<NetworkObject>().OwnerClientId) clientIdsList.Add(player.GetComponent<NetworkObject>().OwnerClientId);
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

    

}
