using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.VisualScripting;


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

    private Slider reloadUISlider;
    private GameObject reloadSliderHolder;

    private Slider chargeUISlider;

    private GameObject chargeSliderHolder;

    List<GameObject> orbs = new List<GameObject>();
    



    private void Start()
    {   
        if (IsLocalPlayer)
        { 
            reloadSliderHolder = GameObject.Find("ReloadUI");
            reloadUISlider = reloadSliderHolder.GetComponent<Slider>();
            reloadUISlider.value = 0f;

            chargeSliderHolder = GameObject.Find("ChargeUI");
            chargeUISlider = chargeSliderHolder.GetComponent<Slider>();
            chargeUISlider.value = 0f;
        }
    }
    void Update()
    {

        if (!IsLocalPlayer) return;

        if (readyToFire == true){
            if (Input.GetKey(fireKey))
            {
                accumulatedTime += Time.deltaTime;

                chargeUISlider.value = Mathf.Min(0.4f, 0.4f * Mathf.Clamp01(accumulatedTime / statsManager.orbChargeTime.Value));
            }

            if(Input.GetKeyUp(fireKey))
            {
                chargeUISlider.value = 0f;
                accumulatedTimeBurst = accumulatedTime;
                readyToFire = false;
                ShootProjectile(accumulatedTime);
                Invoke(nameof(burstshot), GetComponent<PlayerStatsManager>().orbBurstDelay.Value);
                Invoke(nameof(ResetFire), statsManager.orbCooldown.Value);
                StartCoroutine(UpdateReloadUI());
                accumulatedTime = 0f;
            }
        }
    }

    public void resetSliders(){
        chargeUISlider.value = 0f;
        accumulatedTime = 0f;
    }

    private IEnumerator UpdateReloadUI(){
        float elapsedTime = 0f;

        while (elapsedTime < statsManager.orbCooldown.Value)
        {
            elapsedTime += Time.deltaTime;
            reloadUISlider.value = 0.4f - 0.4f*Mathf.Clamp01(elapsedTime / statsManager.orbCooldown.Value);
            yield return null;
        }

        reloadUISlider.value = 0f;
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
        if(GetComponent<PlayerStatsManager>().numberOfOrbs.Value <= 0) return;
        
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

        IgnorePhysics(orbs);
        orbs = new List<GameObject>();

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
        PlayerStatsManager player = GetComponent<PlayerStatsManager>();
        projectileObj.transform.localScale = new Vector3(player.orbScale.Value, player.orbScale.Value, player.orbScale.Value);
        projectileObj.GetComponent<Fireball>().SetDamageStats(player.orbDamage.Value, player.orbKnockbackForce.Value, player.orbKnockbackPercentDamage.Value, player.orbPriority.Value);
        projectileObj.GetComponent<Fireball>().SetExplosionStats(player.explosionDamage.Value, player.explosionKnockbackForce.Value, player.explosionKnockbackPercentDamage.Value,  player.explosionRadius.Value, player.explosionIgnoreOwnerDamage.Value);
        projectileObj.GetComponent<Fireball>().SetSpecialStats(player.homing.Value, player.maxBounces.Value, player.clusterBomb.Value);
        projectileObj.GetComponent<Fireball>().SetStunStats(player.orbSpeedReduction.Value, player.orbAgilityReduction.Value, player.orbStunTimer.Value);
        projectileObj.GetComponent<Fireball>().SetPlayerOwnerId(OwnerClientId);
        orbs.Add(projectileObj);

        projectileObj.GetComponent<NetworkObject>().Spawn(true);
        
        IgnorePhysicsRpc(projectileObj.GetComponent<NetworkObject>().NetworkObjectId, RpcTarget.Single(id, RpcTargetUse.Temp));
        
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firepoint).normalized * speedMod; //* 0.01f;
        ConstantForce constantForce = projectileObj.GetComponent<ConstantForce>();
        projectileObj.GetComponent<ConstantForce>().force = new Vector3(constantForce.force.x, constantForce.force.y * dropMod, constantForce.force.z);

        NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
        Physics.IgnoreCollision(projectileObj.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Body").GetComponent<Collider>());
        Physics.IgnoreCollision(projectileObj.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Head").GetComponent<Collider>());
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void IgnorePhysicsRpc(ulong projectileId, RpcParams rpcParams)
    {
        NetworkObject projectileNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileId];
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        Physics.IgnoreCollision(projectileNetworkObject.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Body").GetComponent<Collider>());
        Physics.IgnoreCollision(projectileNetworkObject.GetComponent<Collider>(), playerNetworkObject.transform.Find("Model/Head").GetComponent<Collider>());
    }

    void IgnorePhysics(List<GameObject> objects)
    {
        for(int i = 0; i < objects.Count; i++)
        {
            for(int j = i+1; j < objects.Count; j++)
            {
                Physics.IgnoreCollision(objects[i].GetComponent<Collider>(), objects[j].GetComponent<Collider>()); 
            }
        }
    }

}
