using UnityEngine;
using Assignment;

//Different to the zipline one, used to teleport the player back when they enter them
public class Water_Teleport : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(player.Respawn(2));
        }
    }
}
