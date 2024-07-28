using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerClusterBomb : NetworkBehaviour
{
    public GameObject projectile;

    private List<Vector3> vectorAngleList;

    public void spawnClusterBombs(Vector3 normal)
    {
        if (!IsServer) return;

        List<GameObject> bombs = new List<GameObject>();

        calculateVectors(GetComponent<PlayerStatsManager>().pulseClusterBomb.Value, normal);

        int numClusterBombs = GetComponent<PlayerStatsManager>().pulseClusterBomb.Value;

        while(numClusterBombs > 0)
        {
            numClusterBombs--;

            Fireball fireball = GetComponent<Fireball>();
            PlayerStatsManager player = GetComponent<PlayerStatsManager>();
            
            GameObject projectileObj = Instantiate(projectile, transform.position, Quaternion.identity);
            projectileObj.transform.localScale = new Vector3(player.orbScale.Value, player.orbScale.Value, player.orbScale.Value);
            projectileObj.GetComponent<Fireball>().SetDamageStats(player.orbDamage.Value, player.orbKnockbackForce.Value, player.orbKnockbackPercentDamage.Value, player.orbPriority.Value);
            projectileObj.GetComponent<Fireball>().SetExplosionStats(player.explosionDamage.Value, player.explosionKnockbackForce.Value, player.explosionKnockbackPercentDamage.Value,  player.explosionRadius.Value, true);
            projectileObj.GetComponent<Fireball>().SetPlayerOwnerId(OwnerClientId);

            Physics.IgnoreCollision(projectileObj.GetComponent<Collider>(), transform.Find("Model/Body").GetComponent<Collider>());
            Physics.IgnoreCollision(projectileObj.GetComponent<Collider>(), transform.Find("Model/Head").GetComponent<Collider>());
            projectileObj.GetComponent<NetworkObject>().Spawn(true);
            IgnorePhysicsRpc(projectileObj.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));

            bombs.Add(projectileObj);

        }

        IgnorePhysics(bombs);

        foreach(GameObject bomb in bombs)
        {
            Physics.IgnoreCollision(bomb.GetComponent<Collider>(), GetComponent<Collider>());
        }

        
        int i = 0;
        foreach(GameObject bomb in bombs)
        {
            bomb.GetComponent<Rigidbody>().velocity = (normal + vectorAngleList[i].normalized * 0.5f).normalized * 10f;
            i++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void IgnorePhysicsRpc(ulong projectileId, RpcParams rpcParams)
    {
        NetworkObject projectileNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileId];
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        Physics.IgnoreCollision(projectileNetworkObject.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Body").GetComponent<Collider>());
        Physics.IgnoreCollision(projectileNetworkObject.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Head").GetComponent<Collider>());
    }

    
    private void calculateVectors(int numPoints, Vector3 normal)
    {
        vectorAngleList = new List<Vector3>();

        Vector3 arbitraryVector = Vector3.right;
        Vector3 perpendicular = FindPointToRay(arbitraryVector, transform.position, normal).normalized;
        Vector3 perpendicular2 = Vector3.Cross(normal, perpendicular).normalized;

        float angleBetween = 6.2831853f/numPoints;

        float randAngle = UnityEngine.Random.Range(0f, angleBetween);

        for(int i = 0; i < numPoints; i++)
        {
            vectorAngleList.Add(perpendicular * Mathf.Cos(angleBetween*i+randAngle) + perpendicular2 * Mathf.Sin(angleBetween*i+randAngle));
        }
    
    }

    private void IgnorePhysics(List<GameObject> objects)
    {
        for(int i = 0; i < objects.Count; i++)
        {
            for(int j = i+1; j < objects.Count; j++)
            {
                Physics.IgnoreCollision(objects[i].GetComponent<Collider>(), objects[j].GetComponent<Collider>()); 
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
