using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBlast : NetworkBehaviour
{

    public float radius = 4f;
    public string playerTag = "Player";
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer){
            Invoke("DestroyProjectile", 5);
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
        foreach (var x in playersInRange) {
            Debug.Log(i + " = " + x.GetComponent<NetworkObject>().OwnerClientId);
            i += 1;
        }
    }
    

}
