using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBlast : NetworkBehaviour
{

    private bool ignoreOwnerDamage = false;
    private ulong playerOwnerId;
    public string playerTag = "Player";

    public GameObject audioSrcPrefab;

    public GameObject gameManager;

    private float explosionRadius;
    private float explosionDamage;
    private float explosionKnockbackPercentDamage;
    private float explosionKnockbackForce;

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


    public void SetStats(float explosionRadiusInput, float explosionDamageInput, float explosionKnockbackPercentDamageInput, float explosionKnockbackForceInput, bool ignoreOwnerDamageInput)
    {
        explosionRadius = explosionRadiusInput;
        explosionDamage = explosionDamageInput;
        explosionKnockbackPercentDamage = explosionKnockbackPercentDamageInput;
        explosionKnockbackForce = explosionKnockbackForceInput;
        ignoreOwnerDamage = ignoreOwnerDamageInput;
    }

    public void SetPlayerWhoFired(ulong playerId)
    {
        playerOwnerId = playerId;
    }


    private void DestroyBlastObject()
    {
        NetworkObject.Despawn();
    }

    private void FindPlayers()
    {

        PlayerStatsManager playerWhoShot = NetworkManager.Singleton.ConnectedClients[playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        List<GameObject> playersInRange = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            GameObject rootObj = hitCollider.transform.root.gameObject;
            if(playersInRange.Contains(rootObj)) continue;
            if(!rootObj.CompareTag(playerTag) && !rootObj.CompareTag("decoy")) continue;
            if(rootObj.CompareTag(playerTag) && ignoreOwnerDamage && playerOwnerId == rootObj.GetComponent<NetworkObject>().OwnerClientId) continue;    

            playersInRange.Add(rootObj);
        }

        List<ulong> clientIdsList = new List<ulong>();
        
        foreach (var player in playersInRange)
        {
            var ray = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;

            if(!Physics.Raycast(ray, out hit)) continue;

            if (hit.collider.transform.root.gameObject.CompareTag("Player"))
            {
                gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, explosionDamage, playerOwnerId);
                gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, explosionKnockbackPercentDamage);
                
                player.GetComponent<PlayerKnockback>().ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, explosionKnockbackForce, false, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));

                if (playerOwnerId != player.GetComponent<NetworkObject>().OwnerClientId) clientIdsList.Add(player.GetComponent<NetworkObject>().OwnerClientId);
            }
            else if(hit.collider.transform.root.gameObject.CompareTag("decoy"))
            {
                hit.collider.transform.root.gameObject.GetComponent<DecoyScript>().DestroyDecoy();
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
