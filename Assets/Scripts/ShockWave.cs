using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShockWave : NetworkBehaviour
{

    private Vector3 initialScale;

    private Renderer objectRenderer;

    private Color initialColor;

    void Start()
    {
        // if (IsServer){
        //     Invoke(nameof(DestroyShockwave), 10f);
        //     initialScale = GetComponent<Transform>().localScale;
        // }

        initialScale = GetComponent<Transform>().localScale;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null) {
            initialColor = objectRenderer.material.color;
        }
        Destroy(gameObject, 0.5f);
    }

    void FixedUpdate()
    {
        if (IsServer){
            //IncreaseScale(0.01f, initialScale);  
            //ShockWaveOpacityRpc(0.3f);
        }
        IncreaseScale(0.85f, initialScale);  
        DecreaseOpacity(1f);
    }


    // private void DestroyShockwave(){
    //     NetworkObject.Despawn();
    // }



    private void IncreaseScale(float scaleFactor, Vector3 initialeScale){
        GetComponent<Transform>().localScale += scaleFactor * initialeScale;
    }

    private void DecreaseOpacity(float fadeAmount) {
        if (objectRenderer != null) {
            Color color = objectRenderer.material.color;
            color.a -= fadeAmount * Time.deltaTime;
            color.a = Mathf.Clamp(color.a, 0f, initialColor.a); // Ensure alpha doesn't go below 0
            objectRenderer.material.color = color;
        }
    }

    // [Rpc(SendTo.Everyone)]
    // private void ShockWaveOpacityRpc(float fadeAmount){
    //     DecreaseOpacity(fadeAmount);
    // }
}
