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
        gameManager = GameObject.Find("GameManager");
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found within scene");
        }
        if (IsServer){
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
            Debug.Log("--------------------------- Break Line -------------------");
            Debug.Log(i + " = " + player.GetComponent<NetworkObject>().OwnerClientId);
            i += 1;

            var ray = new Ray(GetComponent<Transform>().position, player.GetComponent<Transform>().position - GetComponent<Transform>().position);
            RaycastHit hit;

            Debug.Log("Ray = " + ray);

            if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.collider.gameObject;
            Debug.Log("Ray hit object: " + objectHit.name);
                if (objectHit.CompareTag("Player")){
                    gameManager.GetComponent<HealthManager>().applyDamage(player.GetComponent<NetworkObject>().OwnerClientId);
                }
            } else {
                Debug.Log("No hit");
            }
        }

    }
    

}
