using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Homing : NetworkBehaviour
{
    private Vector3 origin;
    private Vector3 direction;
    public GameObject nearestPlayer = null;
    PlayerStatsManager playerWhoShot = null;
    public float homingStrength;

    void Start()
    {
        if (!IsServer) return;
        playerWhoShot = NetworkManager.Singleton.ConnectedClients[GetComponent<Fireball>().playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();
        homingStrength = GetComponent<Fireball>().homing;
        //Invoke(nameof(findNearestPlayer), 0.1f);
    }

    void Update()
    {
        if (!IsServer || homingStrength == 0) return;
        if(nearestPlayer == null)
        {
            findNearestPlayer();
            return;
        }

        

        Vector3 orbToPlayer = nearestPlayer.transform.position - transform.position;
        float orbToPlayerDistance = orbToPlayer.magnitude;
        float orbToPlayerTime = orbToPlayerDistance / GetComponent<Rigidbody>().velocity.magnitude;
        Vector3 playerVelocity = nearestPlayer.GetComponent<Rigidbody>().velocity;
        Vector3 newLocation = nearestPlayer.transform.position + playerVelocity * orbToPlayerTime;



        float distance = (nearestPlayer.transform.position - transform.position).magnitude;
        Vector3 projDistance = FindPointToRay(newLocation, transform.position, GetComponent<Rigidbody>().velocity.normalized);//

        // float DistanceToPlayerPlane = (nearestPlayer.transform.position + projDistance - transform.position).magnitude;
        // float TimetoPlayerPlane = DistanceToPlayerPlane/GetComponent<Rigidbody>().velocity.magnitude;
        // Vector3 projDistance2 = -1*FindPointToRay(nearestPlayer.transform.position, transform.position, GetComponent<Rigidbody>().velocity.normalized);
        
        Vector3 dir = -1*projDistance.normalized;
        float forceMod = (100f/distance + 10f) * 0.01f;
        if(forceMod > 2f) forceMod = 1.5f;
        GetComponent<Rigidbody>().AddForce(dir*homingStrength*forceMod, ForceMode.Acceleration);
    }

    private void findNearestPlayer()
    {
        direction = GetComponent<Rigidbody>().velocity.normalized;
        origin = transform.position;
        Debug.DrawRay(origin, direction*1000f, Color.white, 1000f);
        float minDistance = 1000f;
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if(instance.GetComponent<NetworkObject>().OwnerClientId != playerWhoShot.GetComponent<NetworkObject>().OwnerClientId)
            {
                //Debug.Log("player " + instance.GetComponent<NetworkObject>().OwnerClientId + " distance to ray is" + FindPointToRay(instance.transform.position, origin, direction).magnitude);
                if(FindPointToRay(instance.transform.position, origin, direction).magnitude < minDistance)
                {
                    if(Vector3.Dot(direction, instance.transform.position - origin) > 0) 
                    {
                        nearestPlayer = instance.gameObject;
                        minDistance = FindPointToRay(instance.transform.position, origin, direction).magnitude;
                    }
                }
            }
        }
    }

    private Vector3 FindPointToRay(Vector3 point, Vector3 origin, Vector3 direction)
    {
        Vector3 originToPoint = point - origin;

        Vector3 projection = Vector3.Project(originToPoint, direction);

        Vector3 pointToRay = projection - originToPoint;

        return pointToRay;
    }

}
