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


    public void SetHomingStats(Vector3 originInput, Vector3 directionInput)
    {
        origin = originInput;
        direction = directionInput;
    }

    void Start()
    {
        if (!IsServer) return;
        playerWhoShot = NetworkManager.Singleton.ConnectedClients[GetComponent<Fireball>().playerOwnerId].PlayerObject.GetComponent<PlayerStatsManager>();
        homingStrength = GetComponent<Fireball>().homing;
        findNearestPlayer();
    }

    void Update()
    {
        if (!IsServer || nearestPlayer == null || homingStrength == 0) return;

        Vector3 dir = -1f * FindPointToRay(nearestPlayer.transform.position, transform.position, GetComponent<Rigidbody>().velocity.normalized).normalized;
        GetComponent<Rigidbody>().AddForce(dir*homingStrength, ForceMode.Acceleration);
    }

    private GameObject findNearestPlayer()
    {
        float minDistance = 1000f;
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            if(instance.GetComponent<NetworkObject>().OwnerClientId != playerWhoShot.GetComponent<NetworkObject>().OwnerClientId)
            {
                if(FindPointToRay(instance.transform.position, origin, direction).magnitude < minDistance)
                {
                    if(Vector3.Dot(direction, instance.transform.position - origin) > 0) nearestPlayer = instance.gameObject;
                }
            }
        }
        return nearestPlayer;
    }

    private Vector3 FindPointToRay(Vector3 point, Vector3 origin, Vector3 direction)
    {
        Vector3 originToPoint = point - origin;

        Vector3 projection = Vector3.Project(originToPoint, direction);

        Vector3 pointToRay = projection - originToPoint;

        return pointToRay;
    }

}
