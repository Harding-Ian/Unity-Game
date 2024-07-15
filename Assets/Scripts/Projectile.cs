using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Projectile : NetworkBehaviour
{
    public Camera cam;

    public GameObject projectile;

    [Header("ProjectileButton")]
    public KeyCode fireKey = KeyCode.Mouse0;

    public PlayerStatsManager statsManager;

    private bool readyToFire = true;

    private float accumulatedTime = 0f;
    private float accumulatedTimeBurst;
    private int burstIteration = 1;
    

    void Update()
    {

        if (!IsLocalPlayer) return;

        if (readyToFire == true){
            if (Input.GetKey(fireKey))
            {
                accumulatedTime += Time.deltaTime;
            }

            if(Input.GetKeyUp(fireKey))
            {
                accumulatedTimeBurst = accumulatedTime;
                readyToFire = false;
                ShootProjectile(accumulatedTime);
                Invoke(nameof(burstshot), GetComponent<PlayerStatsManager>().orbBurstDelay.Value);
                Invoke(nameof(ResetFire), statsManager.orbCooldown.Value);
                accumulatedTime = 0f;
            }

            if(Input.GetKeyDown(KeyCode.M))
            {
                GetComponent<PlayerStatsManager>().orbPriority.Value += 1;
                Debug.Log("orb priority increased to" + GetComponent<PlayerStatsManager>().orbPriority.Value);
            }
        }
    }


    void burstshot()
    {
        if(burstIteration < GetComponent<PlayerStatsManager>().orbBurst.Value)
        {
            ShootProjectile(accumulatedTimeBurst);
            Invoke(nameof(burstshot), GetComponent<PlayerStatsManager>().orbBurstDelay.Value);
            burstIteration++;
        }
        else
        {
            burstIteration = 1;
        }
    }

    void ShootProjectile(float pressTime)
    {
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 destination;
        RaycastHit hit;
        List<Vector3> firepoints = new List<Vector3>();
        List<Vector3> hitpoints = new List<Vector3>();

        if(Physics.Raycast(ray, out hit)) destination = hit.point;
        else destination = ray.GetPoint(1000);

        GetComponent<PlayerMultishot>().calculateMultiShot(destination, out firepoints, out hitpoints);

        float dropMod = 1f;
        float speedMod = calculateChargeBonus(pressTime, out dropMod);

        int i = 0;
        foreach(Vector3 firepoint in firepoints)
        {
            ProjectileRpc(OwnerClientId, firepoint, hitpoints[i], dropMod, speedMod);
            i++;
        }

    }

    private void ResetFire(){
        readyToFire = true;
    }

    private float calculateChargeBonus(float pressTime, out float dropMod){
        float range = statsManager.orbMaxSpeed.Value - statsManager.orbMinSpeed.Value;

        if (statsManager.orbChargeTime.Value < pressTime){
            dropMod = 0.5f;
            return statsManager.orbMaxSpeed.Value;
        }
        else{
            dropMod = 1f - 0.5f * (pressTime / statsManager.orbChargeTime.Value);
            return statsManager.orbMinSpeed.Value + range * (pressTime / statsManager.orbChargeTime.Value);
        }
    }


    [Rpc(SendTo.Server)]
    private void ProjectileRpc(ulong id, Vector3 firepoint, Vector3 destination, float dropMod, float speedMod)
    {
        GameObject projectileObj = Instantiate(projectile, firepoint, Quaternion.identity);
        projectileObj.transform.localScale = new Vector3(statsManager.orbScale.Value,statsManager.orbScale.Value,statsManager.orbScale.Value);
        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        
        
        IgnorePhysicsRpc(projectileObj.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(id, RpcTargetUse.Temp));
        
        projectileObj.GetComponent<Fireball>().SetPlayerWhoFired(OwnerClientId);
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firepoint).normalized * speedMod; //* 0.01f;

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
