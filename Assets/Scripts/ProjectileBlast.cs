using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBlast : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer){
            Invoke("DestroyProjectile", 5);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void DestroyProjectile()
    {
        NetworkObject.Despawn();
    }

}
