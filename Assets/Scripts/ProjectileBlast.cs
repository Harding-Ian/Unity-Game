using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBlast : NetworkBehaviour
{

    public float radius = 4f;
    public string playerTag = "Player";

    public GameObject gameManager;
    // Start is called before the first frame update
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

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }

    private void FindPlayers(){
        Vector3 blastCenter = transform.position;

        Collider[] hitColliders = Physics.OverlapSphere(blastCenter, radius);

        List<GameObject> playersInRange = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(playerTag))
            {
                playersInRange.Add(hitCollider.gameObject);
            }
        }

        Debug.Log("Players in range = " + playersInRange.Count);
        int i = 0;
        foreach (var player in playersInRange) {
            // Debug.Log("--------------------------- Break Line -------------------");
            // Debug.Log(i + " = " + player.GetComponent<NetworkObject>().OwnerClientId);
            i += 1;

            var ray = new Ray(GetComponent<Transform>().position, player.GetComponent<Transform>().position - GetComponent<Transform>().position);
            RaycastHit hit;
            
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider.bounds.Contains(ray.origin)) {
                gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, 1f);
                ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, 0.1f);
            }
            else if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.collider.gameObject;
                if (objectHit.CompareTag("Player")){
                    gameManager.GetComponent<StatsManager>().ApplyDamage(player.GetComponent<NetworkObject>().OwnerClientId, 1f);
                    ApplyKnockbackRpc((player.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized, RpcTarget.Single(player.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
                    gameManager.GetComponent<StatsManager>().UpdateKnockback(player.GetComponent<NetworkObject>().OwnerClientId, 0.1f);
                }
                //Debug.Log("objhect hit == " + hit.collider.gameObject.name);
            }

        }
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
