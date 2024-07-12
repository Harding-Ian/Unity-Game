using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMultishot : NetworkBehaviour
{



    public void calculateMultiShot(Vector3 hitpoint, out List<Vector3> firepoints, out List<Vector3> hitpoints)
    {
        firepoints = new List<Vector3>();
        hitpoints  = new List<Vector3>();

        Vector3 firepoint = transform.Find("FirePoint").position;

        if (GetComponent<PlayerStatsManager>().fireShape.Value.ToString() == "volley")
        {
            float gap =  0.15f;
            float separation = 0.7f + gap;
            int remainingOrbs = GetComponent<PlayerStatsManager>().numberOfOrbs.Value;

            Vector3 Lfirepoint;
            Vector3 Rfirepoint;
            Vector3 Lhitpoint;
            Vector3 Rhitpoint;

            if(remainingOrbs % 2 == 0) // even
            {
                float offset = separation / 2f;


                Lfirepoint = firepoint - offset * transform.right.normalized;
                Rfirepoint = firepoint + offset * transform.right.normalized;
                Lhitpoint = hitpoint - offset * transform.right.normalized;
                Rhitpoint = hitpoint + offset * transform.right.normalized;


                firepoints.Add(Lfirepoint);
                firepoints.Add(Rfirepoint);
                hitpoints.Add(Lhitpoint);
                hitpoints.Add(Rhitpoint);
                remainingOrbs -= 2;
            }
            else // odd
            {
                Lfirepoint = firepoint;
                Rfirepoint = firepoint;
                Lhitpoint = hitpoint;
                Rhitpoint = hitpoint;

                firepoints.Add(firepoint);
                hitpoints.Add(hitpoint);
                remainingOrbs -= 1;
            }

            

            for(int i = 1; i <= remainingOrbs / 2; i++)
            {
                Vector3 nextLfirepoint = firepoint - separation * i * transform.right.normalized;
                Vector3 nextRfirepoint = firepoint + separation * i * transform.right.normalized;
                Vector3 nextLhitpoint = hitpoint - separation * i * transform.right.normalized;
                Vector3 nextRhitpoint = hitpoint + separation * i * transform.right.normalized;
                
                firepoints.Add(nextLfirepoint);
                firepoints.Add(nextRfirepoint);
                hitpoints.Add(nextLhitpoint);
                hitpoints.Add(nextRhitpoint);
            }   
        }


        else if (GetComponent<PlayerStatsManager>().fireShape.Value.ToString() == "default")
        {
            firepoints.Add(firepoint);
            hitpoints.Add(hitpoint);
        }

        
        else if (GetComponent<PlayerStatsManager>().fireShape.Value.ToString() == "cluster")
        {
            firepoints.Add(firepoint);
            hitpoints.Add(hitpoint);
        }

    }



}
