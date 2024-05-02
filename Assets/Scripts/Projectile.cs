using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Projectile : NetworkBehaviour
{
    public Camera cam;

    // public struct FirePointData{
    //     public Vector3 position;
    //     public Quaternion rotation;
        
    // }
    private Vector3 destination;

    public GameObject projectile;
    public Transform LHFirePoint, RHFirePoint;
    //private bool leftHand;

    public float projectileSpeed;

    [Header("ProjectileButton")]
    public KeyCode fireKey = KeyCode.Mouse0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(fireKey))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (!IsOwner) return;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)){
            destination = hit.point;
            Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            Debug.Log("Hit Point: " + hit.point);
        }else{
            destination = ray.GetPoint(1000);
        }

        // FirePointData firePointData= new FirePointData();
        // firePointData.position = RHFirePoint.position;
        // firePointData.rotation = RHFirePoint.rotation;


        //InstantiateProjectile(RHFirePoint);
        ProjectileServerRpc();
    }

    void InstantiateProjectile(Transform firePoint){
        GameObject projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity);
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
    }

    [ServerRpc]
    private void ProjectileServerRpc(){
        GameObject projectileObj = Instantiate(projectile, RHFirePoint.position, Quaternion.identity);
        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - RHFirePoint.position).normalized * projectileSpeed;
    }
}
