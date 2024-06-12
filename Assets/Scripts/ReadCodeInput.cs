using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCodeInput : MonoBehaviour
{

    public NetworkRelay networkRelay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadStringInput(string str){
        Debug.Log("input --> " + str);
        networkRelay.JoinRelay(str);
    }
}
