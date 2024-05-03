using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Projectile : MonoBehaviour
{
    public Camera cam;

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
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)){
            destination = hit.point;
            Debug.Log("Hit Object: " + hit.collider.gameObject.name);
            Debug.Log("Hit Point: " + hit.point);
        }else{
            destination = ray.GetPoint(1000);
        }

        InstantiateProjectile(RHFirePoint);
    }

    void InstantiateProjectile(Transform firePoint){
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
    }
}
