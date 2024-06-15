using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Projectile : NetworkBehaviour
{
    public Camera cam;

    public GameObject projectile;
    public Transform FirePoint;

    [Header("ProjectileButton")]
    public KeyCode fireKey = KeyCode.Mouse0;

    public PlayerStatsManager statsManager;

    private bool readyToFire = true;

    private float accumulatedTime = 0f;

    private bool fireTest = true;
    
    public float maxChargeTime = 1f;

    // Start is called before the first frame update
    // void Start()
    // {
    // }

    // Update is called once per frame
    void Update()
    {
        if(fireTest == true){
            fireTest = false;
        }

        if (!IsLocalPlayer) return;

        if (readyToFire == true){
            if (Input.GetKey(fireKey))
            {
                accumulatedTime += Time.deltaTime;
            }

            if(Input.GetKeyUp(fireKey))
            { 
                readyToFire = false;
                ShootProjectile(accumulatedTime);
                Invoke(nameof(ResetFire), statsManager.projectileCooldown.Value);
                accumulatedTime = 0f;
            }
        }
    }


    void ShootProjectile(float pressTime)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ProjectileRpc(ray, OwnerClientId, pressTime);
    }

    private void ResetFire(){
        readyToFire = true;
        fireTest = true;
    }

    private float calculateChargeBonus(float pressTime, out float dropMod){
        float range = statsManager.maxProjectileSpeed.Value - statsManager.minProjectileSpeed.Value;

        if (statsManager.projectileChargeTime.Value < pressTime){
            dropMod = 0.5f;
            return statsManager.maxProjectileSpeed.Value;
        }
        else{
            dropMod = 1f - 0.5f * (pressTime / statsManager.projectileChargeTime.Value);
            return statsManager.minProjectileSpeed.Value + range * (pressTime / statsManager.projectileChargeTime.Value);
        }
    }


    [Rpc(SendTo.Server)]
    private void ProjectileRpc(Ray ray, ulong id, float pressTime)
    {
        
        Vector3 destination;

        RaycastHit hit;


        float dropMod = 1f;
        float speedMod = calculateChargeBonus(pressTime, out dropMod);


        if(Physics.Raycast(ray, out hit)){
            destination = hit.point;
        }else{
            destination = ray.GetPoint(1000);
        }


        GameObject projectileObj = Instantiate(projectile, FirePoint.position, Quaternion.identity);
        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        
        IgnorePhysicsRpc(projectileObj.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(id, RpcTargetUse.Temp));
        
        projectileObj.GetComponent<Fireball>().SetPlayerWhoFired(OwnerClientId);
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - FirePoint.position).normalized * speedMod; //* 0.01f;

        ConstantForce constantForce = projectileObj.GetComponent<ConstantForce>();


        projectileObj.GetComponent<ConstantForce>().force = new Vector3(constantForce.force.x, constantForce.force.y * dropMod, constantForce.force.z);

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
