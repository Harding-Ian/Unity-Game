using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerKnockback : NetworkBehaviour
{
    public PlayerStatsManager statsManager;

    [Rpc(SendTo.SpecifiedInParams)]
    public void ApplyKnockbackRpc(Vector3 knockbackDirection, float knockbackForce, bool invert, RpcParams rpcParams)
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        
        if(invert) knockbackDirection *= -1f;

        float angle = 180 - Vector3.Angle(knockbackDirection, Vector3.down) - 65;

        if (angle < 0) {angle = 0;}

        float adjustedAngle = (angle / 115f) * 30f;
        
        float adjustedRadians = (adjustedAngle * 3.1415f) / 180f;

        Vector3 adjustedknockbackDirection = Vector3.RotateTowards(knockbackDirection, Vector3.up, adjustedRadians, 1);

        GetComponent<Rigidbody>().AddForce(adjustedknockbackDirection.normalized * knockbackForce * statsManager.knockbackBuildUp.Value * statsManager.knockbackMultiplier.Value, ForceMode.VelocityChange);
    }


}
