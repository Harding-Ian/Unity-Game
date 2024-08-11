using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BillBoardRotation : NetworkBehaviour
{
    private ulong instanceId;
    private ulong playerToLookAtId;
    [SerializeField]
    private bool bigGuy = false;

    private float speed = 2f;

    void Start(){
        if(bigGuy){
            StartCoroutine(MoveForwardCoroutine());
        }
    }

    private IEnumerator MoveForwardCoroutine()
    {
        float duration = 2f; // Duration to move forward
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.Translate(new Vector3(0,0,1) * speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void LateUpdate()
    {
        if(IsLocalPlayer) return;

        playerToLookAtId = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerDeath>().playerSpectatingId.Value;

        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            instanceId = instance.GetComponent<PlayerScript>().clientId.Value;
            if (instanceId == playerToLookAtId)
            {
                if(bigGuy) 
                {
                    transform.LookAt(instance.transform);
                    //transform.rotation = instance.transform.rotation;
                }
                else 
                {
                    //transform.LookAt(instance.transform);
                    transform.rotation = instance.transform.Find("CameraHolder").rotation;
                }
                return;
            }
        }
    }
}
