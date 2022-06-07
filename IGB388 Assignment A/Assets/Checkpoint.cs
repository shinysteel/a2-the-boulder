using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assignment;

public class Checkpoint : MonoBehaviour
{
    public GameObject waypoint;
    Player player;
    public List<GameObject> special_waypoints = new List<GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.respawn_location = waypoint.transform;

            if (!special_waypoints.Contains(waypoint))
            {
                gameObject.SetActive(false);
            }

            if (name == "Checkpoint 8")
            {
                foreach (GameObject waypoint in special_waypoints)
                {
                    waypoint.SetActive(false);
                }
            }
        }
    }
}
