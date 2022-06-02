using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    //Fetch zipline for check
    public Zipline zipline;

    //Locations for spawning player if fallen in
    public Transform spawn_location_a;
    public Transform spawn_location_b;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //If zipline has been used
            if (zipline.finished_zipline)
            {
                other.gameObject.transform.position = spawn_location_b.position;
            }

            //If zipline has not been used
            else
            {
                other.gameObject.transform.position = spawn_location_a.position;
            }
        }
    }
}
