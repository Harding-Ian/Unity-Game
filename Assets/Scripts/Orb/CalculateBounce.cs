using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CalculateBounce : NetworkBehaviour
{
    public Vector3 BounceDirection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
        List<GameObject> playersInView = new List<GameObject>();
        GameObject closestPlayer = null;
        float closestdistance = 20f;

        bool runCode = false;

        foreach (Collider hitCollider in hitColliders)
        {
            runCode = false;
            if(hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId != GetComponent<Fireball>().playerOwnerId) runCode = true;
            else if(hitCollider.transform.root.CompareTag("decoy") && hitCollider.transform.root.GetComponent<DecoyScript>().playerOwnerId !=  GetComponent<Fireball>().playerOwnerId) runCode = true;

            if (!runCode) continue;

            var ray = new Ray(transform.position, hitCollider.gameObject.transform.position - transform.position);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit) && (hit.collider.transform.root.CompareTag("Player") || hit.collider.transform.root.CompareTag("decoy")) && hit.distance < closestdistance)
            {
                closestPlayer = hitCollider.transform.root.gameObject;
                closestdistance= hit.distance;
            }


            // if(!hitCollider.transform.root.CompareTag("Player") && !hitCollider.transform.root.CompareTag("decoy")) continue;
            // if(hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId == GetComponent<Fireball>().playerOwnerId) continue;
            // if(hitCollider.transform.root.CompareTag("decoy") && hitCollider.transform.root.GetComponent<DecoyScript>().playerOwnerId ==  GetComponent<Fireball>().playerOwnerId) continue;

            // var ray = new Ray(transform.position, hitCollider.gameObject.transform.position - transform.position);
            // RaycastHit hit;

            // if(!Physics.Raycast(ray, out hit)) continue;
            // if(!hit.collider.transform.root.CompareTag("Player") || !hit.collider.transform.root.CompareTag("decoy")) continue;
            // if(hit.distance > closestdistance) continue;

            // closestPlayer = hitCollider.transform.root.gameObject;
            // closestdistance= hit.distance;

        }
        
        if (closestPlayer == null) return Vector3.zero;

        return (closestPlayer.transform.position - transform.position).normalized;
    }

    // private GameObject NearestPlayer()
    // {
    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
    //     List<GameObject> playersInView = new List<GameObject>();
    //     GameObject closestPlayer = null;
    //     float closestdistance = 20f;

    //     foreach (Collider hitCollider in hitColliders)
    //     {
    //         if (hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId != playerOwnerId)
    //         {
    //             var ray = new Ray(transform.position, hitCollider.gameObject.transform.position - transform.position);
    //             RaycastHit hit;

    //             if (Physics.Raycast(ray, out hit) && hit.collider.transform.root.CompareTag("Player") && hit.distance < closestdistance)
    //             {
    //                 closestPlayer = hitCollider.transform.root.gameObject;
    //                 closestdistance = hit.distance;
    //             }
    //         }
    //     }
        
    //     return closestPlayer;
    // }
}
