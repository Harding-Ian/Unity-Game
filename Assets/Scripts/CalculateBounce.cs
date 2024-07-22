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

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform.root.CompareTag("Player") && hitCollider.transform.root.GetComponent<NetworkObject>().OwnerClientId != GetComponent<Fireball>().playerOwnerId)
            {
                var ray = new Ray(transform.position, hitCollider.gameObject.transform.position - transform.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.transform.root.CompareTag("Player") && hit.distance < closestdistance)
                {
                    closestPlayer = hitCollider.transform.root.gameObject;
                    closestdistance= hit.distance;
                }
            }
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
