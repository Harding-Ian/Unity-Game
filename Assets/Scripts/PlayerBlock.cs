using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBlock : NetworkBehaviour
{


    [Header("BlockButton")]
    public KeyCode blockKey = KeyCode.Mouse1;

    private bool readyToBlock = true;

    public PlayerStatsManager statsManager;

    public GameObject blockWave;

    public GameObject decoyPrefab;
    
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
    private void BlockRpc(ulong id, Vector3 blockOrigin, float radius)
    {
        //GameObject blockObject = Instantiate(blockWave, GetComponent<Transform>().position, Quaternion.identity);
        if(statsManager.decoy.Value == true) spawnDecoy();
        if(statsManager.pulseClusterBomb.Value > 0) GetComponent<PlayerClusterBomb>().spawnClusterBombs(Vector3.up);
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
            var ray = new Ray(transform.position, player.transform.position - transform.position);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit)) 
            {
                GameObject objectHit = hit.collider.transform.root.gameObject;
                if (objectHit.CompareTag("Player"))
                {
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, GetComponent<PlayerStatsManager>().pulseDamage.Value, id);
                    
                    player.GetComponent<PlayerKnockback>().ApplyKnockbackRpc((player.transform.position - transform.position).normalized, GetComponent<PlayerStatsManager>().pulseKnockbackForce.Value, GetComponent<PlayerStatsManager>().pulseInvertKnockback.Value, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, GetComponent<PlayerStatsManager>().pulseKnockbackPercentDamage.Value);
                }
            }

        }
        
    }

    [Rpc(SendTo.Everyone)]
    private void SpawnBlockWaveRpc(Vector3 blockOrigin){
        GameObject blockObject = Instantiate(blockWave, blockOrigin, Quaternion.identity);
    }


    private void ignorePhysics(GameObject player1, GameObject player2)
    {
        Physics.IgnoreCollision(player1.transform.Find("Model/Body").GetComponent<Collider>(), player2.transform.Find("Model/Body").GetComponent<Collider>());
        Physics.IgnoreCollision(player1.transform.Find("Model/Head").GetComponent<Collider>(), player2.transform.Find("Model/Body").GetComponent<Collider>());
        Physics.IgnoreCollision(player1.transform.Find("Model/Body").GetComponent<Collider>(), player2.transform.Find("Model/Head").GetComponent<Collider>());
        Physics.IgnoreCollision(player1.transform.Find("Model/Head").GetComponent<Collider>(), player2.transform.Find("Model/Head").GetComponent<Collider>());
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ignorePhysicsDecoyRpc(ulong decoyNetworkId, RpcParams rpcParams)
    {
        GameObject decoyObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[decoyNetworkId].gameObject;
        GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject;

        ignorePhysics(decoyObject, playerObject);
    }


    private void spawnDecoy()
    {
        PlayerStatsManager player = GetComponent<PlayerStatsManager>();
        GameObject decoy = Instantiate(decoyPrefab, transform.position, transform.rotation);

        decoy.GetComponent<DecoyScript>().SetMovementStats(statsManager.groundMoveForce.Value, statsManager.groundedMoveSpeed.Value, transform.forward, GetComponent<Rigidbody>().rotation);
        decoy.GetComponent<DecoyScript>().SetExplosionStats(player.explosionRadius.Value, player.explosionDamage.Value, player.explosionKnockbackPercentDamage.Value, player.explosionKnockbackForce.Value, player.explosionIgnoreOwnerDamage.Value);
        decoy.GetComponent<DecoyScript>().SetPlayerOwnerId(OwnerClientId);

        decoy.transform.Find("VisibleHealthBarCanvas/VisibleHealthBar").GetComponent<Slider>().value = transform.Find("VisibleHealthBarCanvas/VisibleHealthBar").GetComponent<Slider>().value;

        ignorePhysics(decoy, this.gameObject);
        decoy.GetComponent<NetworkObject>().Spawn(true);

        ignorePhysicsDecoyRpc(decoy.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
        setDecoyColourRpc(transform.Find("Model/Body").GetComponent<Renderer>().materials[0].color, transform.Find("Model/Body").GetComponent<Renderer>().materials[2].color, decoy.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [Rpc(SendTo.Everyone)]
    public void setDecoyColourRpc(Color colour1, Color colour2, ulong decoyNetworkId)
    {
        GameObject decoy = NetworkManager.Singleton.SpawnManager.SpawnedObjects[decoyNetworkId].gameObject;
        Renderer renderer = decoy.transform.Find("Model/Body").GetComponent<Renderer>();
        Renderer renderer2 = decoy.transform.Find("Model/Hat").GetComponent<Renderer>();
        
        renderer.materials[0].color = colour1;
        renderer.materials[2].color = colour2;

        renderer2.materials[0].color = colour1;
    }

}
