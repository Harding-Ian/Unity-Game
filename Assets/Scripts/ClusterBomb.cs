using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClusterBomb : NetworkBehaviour
{
    public GameObject projectile;

    private List<Vector3> vectorAngleList;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void spawnClusterBombs(Vector3 normal)
    {
        if (!IsServer) return;

        Debug.Log("cluster bomb spawned");

        List<GameObject> bombs = new List<GameObject>();

        calculateVectors(GetComponent<Fireball>().clusterBomb, normal);

        while(GetComponent<Fireball>().clusterBomb > 0)
        {
            GetComponent<Fireball>().clusterBomb -= 1;

            GameObject projectileObj = Instantiate(projectile, transform.position + normal.normalized * 0.3f, Quaternion.identity);
            projectileObj.GetComponent<Fireball>().SetPlayerWhoFired(GetComponent<Fireball>().playerOwnerId);
            projectileObj.GetComponent<Fireball>().maxBounces = 0;
            projectileObj.GetComponent<NetworkObject>().Spawn(true);

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

    
    private void calculateVectors(int numPoints, Vector3 normal)
    {
        vectorAngleList = new List<Vector3>();

        Vector3 arbitraryVector = Vector3.right;
        Vector3 perpendicular = FindPointToRay(arbitraryVector, transform.position, normal).normalized;
        Vector3 perpendicular2 = Vector3.Cross(normal, perpendicular).normalized;
        
        Debug.Log("Is it perp???" + Vector3.Dot(normal, perpendicular));
        Debug.Log("Is it perp???" + Vector3.Dot(normal, perpendicular2));
        Debug.Log("Is it perp???" + Vector3.Dot(perpendicular, perpendicular2));

        float angleBetween = 6.2831853f/numPoints;
        
        Debug.Log("Angle between = " + angleBetween);

        for(int i = 0; i < numPoints; i++)
        {
            vectorAngleList.Add(perpendicular * Mathf.Cos(angleBetween*i) + perpendicular2 * Mathf.Sin(angleBetween*i));
            Debug.Log("i=" + i + " total vector  " + (perpendicular * Mathf.Cos(angleBetween*i) + perpendicular2 * Mathf.Sin(angleBetween*i)));
            Debug.Log("i=" + i + " cos component " + perpendicular * Mathf.Cos(angleBetween*i));
            Debug.Log("i=" + i + " sin component " + perpendicular2 * Mathf.Sin(angleBetween*i));
            
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
