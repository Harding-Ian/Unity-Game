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

    public PlayerStatsManager statsManager;

    private bool readyToFire = true;

    // Start is called before the first frame update
    // void Start()
    // {  

    // }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKeyDown(fireKey))
        {
            if (readyToFire == true){
                readyToFire = false;
                ShootProjectile();
                Invoke(nameof(ResetFire), statsManager.projectileCooldown.Value);
            }
        }
    }

    void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ProjectileRpc(ray, OwnerClientId);
    }

    private void ResetFire(){
        readyToFire = true;
    }


    [Rpc(SendTo.Server)]
    private void ProjectileRpc(Ray ray, ulong id){

        Vector3 destination;

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit)){
            destination = hit.point;
        }else{
            destination = ray.GetPoint(1000);
        }


        GameObject projectileObj = Instantiate(projectile, FirePoint.position, Quaternion.identity);
        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        
        IgnorePhysicsRpc(projectileObj.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(id, RpcTargetUse.Temp));
        
        Fireball fireballScript = projectileObj.GetComponent<Fireball>();


        fireballScript.SetPlayerWhoFired(OwnerClientId);

        projectileObj.GetComponent<Rigidbody>().velocity = (destination - FirePoint.position).normalized * projectileSpeed; //* 0.01f;


        NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
        Physics.IgnoreCollision(projectileObj.GetComponent<Collider>(), playerNetworkObject.GetComponent<Collider>());

    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void IgnorePhysicsRpc(ulong projectileId, RpcParams rpcParams)
    {
        NetworkObject projectileNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileId];
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        Physics.IgnoreCollision(projectileNetworkObject.GetComponent<Collider>(), playerNetworkObject.GetComponent<Collider>());
    }
}
