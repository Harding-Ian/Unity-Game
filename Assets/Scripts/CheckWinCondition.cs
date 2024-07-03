using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWinCondition : MonoBehaviour
{
    int alivePlayers;   
    bool isDead;
    void Update()
    {
        alivePlayers = CountAlivePlayers();
        if(alivePlayers == 1)
        {
            Debug.Log("game over geegee");
        }
    }



    int CountAlivePlayers()
    {
        int alivePlayers = 0;
        foreach (var instance in FindObjectsByType<PlayerScript>(FindObjectsSortMode.None))
        {
            isDead = instance.GetComponent<PlayerScript>().dead.Value;
            if(!isDead) 
            {
                alivePlayers++;
            }
        }
        return alivePlayers;
    }



}


