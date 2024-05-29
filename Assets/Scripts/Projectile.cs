using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Projectile : NetworkBehaviour
{
    public Camera cam;

    public GameObject projectile;
    public Transform FirePoint;

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
        ProjectileServerRpc(ray);
    }


    [ServerRpc]
    private void ProjectileServerRpc(Ray ray){

        Vector3 destination;

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)){
            destination = hit.point;
        }else{
            destination = ray.GetPoint(1000);
        }


        GameObject projectileObj = Instantiate(projectile, FirePoint.position, Quaternion.identity);
        projectileObj.GetComponent<NetworkObject>().Spawn(true);


        Fireball fireballScript = projectileObj.GetComponent<Fireball>();
        fireballScript.SetPlayerOwner(OwnerClientId);

        projectileObj.GetComponent<Rigidbody>().velocity = (destination - FirePoint.position).normalized * projectileSpeed; //* 0.01f;
    }
}
