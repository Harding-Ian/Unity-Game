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

        float orbSize = GetComponent<PlayerStatsManager>().orbScale.Value;
        int remainingOrbs = GetComponent<PlayerStatsManager>().numberOfOrbs.Value;

        if (GetComponent<PlayerStatsManager>().fireShape.Value.ToString() == "volley")
        {
            float gap =  0.1f;
            float separation = orbSize + gap;

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
                Vector3 nextLfirepoint = Lfirepoint - separation * i * transform.right.normalized;
                Vector3 nextRfirepoint = Rfirepoint + separation * i * transform.right.normalized;
                Vector3 nextLhitpoint = Lhitpoint - separation * i * transform.right.normalized;
                Vector3 nextRhitpoint = Rhitpoint + separation * i * transform.right.normalized;
                
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
            // List<Vector3> points = new List<Vector3>();

            // points.Add(firepoint);

            // orbSize += 0.05f;


            // float rand = Random.Range(-orbSize, orbSize);
            // bool vertical  = (Random.value > 0.5f);
            // bool side  = (Random.value > 0.5f);


            // if(vertical)
            // {
            //     Vector3 randVec = firepoint + transform.right.normalized * rand;
            //     if(side) points.Add(randVec + transform.up.normalized * orbSize);
            //     else points.Add(randVec - transform.up.normalized * orbSize);
            // }
            // else
            // {
            //     Vector3 randVec = firepoint + transform.up.normalized * rand;
            //     if(side) points.Add(randVec + transform.right.normalized * orbSize);
            //     else points.Add(randVec - transform.right.normalized * orbSize);
            // }

            // for(int i = 3; i <= remainingOrbs; i++)
            // {
            //     for(int j = i+1; j <= remainingOrbs; j++)
            //     {
            //         List<Vector3> possiblepoints = new List<Vector3>();
            //     }
            // }

            // List<Vector3> points = new List<Vector3>();
            // points.Add(firepoint);
            // orbSize += 0.05f;

            // float rand = Random.Range(0f, 1f);
            



            firepoints.Add(firepoint);
            hitpoints.Add(hitpoint);
            // firepoints.Add(points[1]);
            // hitpoints.Add(hitpoint + points[1] - firepoint);
        }

    }
}