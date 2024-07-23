using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClusterBomb : NetworkBehaviour
{
    
    public GameObject projectile;

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

        while(GetComponent<Fireball>().clusterBombs > 0)
        {
            GetComponent<Fireball>().clusterBombs -= 1;

            GameObject projectileObj = Instantiate(projectile, transform.position + normal.normalized * 5f, Quaternion.identity);
            projectileObj.GetComponent<Fireball>().SetPlayerWhoFired(GetComponent<Fireball>().playerOwnerId);

            bombs.Add(projectileObj);

        }

        IgnorePhysics(bombs);

        foreach(GameObject bomb in bombs){
            Physics.IgnoreCollision(bomb.GetComponent<Collider>(), GetComponent<Collider>());
        }

        calculateVectors(bombs);

        foreach(GameObject bomb in bombs){
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<Rigidbody>().velocity = normal * 10f;
        }
    }

    
    private void calculateVectors(List<GameObject> objects){
        
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
