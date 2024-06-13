using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundEffectPlayer : NetworkBehaviour
{

    public AudioSource src;

    public AudioClip directHitSound;
    public AudioClip indirectHitSound;
    // Start is called before the first frame update
    void Start()
    {
        GameObject audioSourceObject = GameObject.Find("AudioSrc");
        if (audioSourceObject != null)
        {
            src = audioSourceObject.GetComponent<AudioSource>();
            if (src == null)
            {
                Debug.LogError("AudioSource component not found on AudioSrc GameObject.");
            }
            else{
                Debug.Log("Src found");
            }
        }
        else
        {
            Debug.LogError("AudioSrc GameObject not found in the scene.");
        }
    }

    // // Update is called once per frame

    public void onDirectHit(){
        src.clip = directHitSound;
        src.Play();
    }

    public void onIndirectHit(){
        src.clip = indirectHitSound;
        src.Play();
    }
}
