using UnityEngine;
using Assignment;

public class Zipline : MonoBehaviour
{
    //Set locations of start and end of zipline
    public Transform start_location;
    public Transform end_location;

    //Get player component
    GameObject player;

    //Variables to determine speed and distance travelled
    public float speed = 1f;
    float start_time;
    float journey_length;

    //Boolean value to enable and disable zipline event
    bool ziplining = false;

    //Boolean value to determine which place to spawn player if fallen in
    public bool finished_zipline = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (ziplining)
        {
            //Distance moved equals elapsed time times speed..
            float distance = (Time.time - start_time) * speed;

            //Fraction of journey completed equals current distance divided by total distance
            float distance_ratio = distance / journey_length;

            //Set our position as a fraction of the distance between the markers
            player.transform.position = Vector3.Lerp(start_location.position, end_location.position, distance_ratio);

            //Stop ziplining once player has reached end_location
            if (distance_ratio == 1)
            {
                //Disable ziplining event
                ziplining = false;

                //Re-enable movement and climbing on player
                player.GetComponent<PlayerMovement>().enabled = true;
                player.GetComponent<PlayerClimbing>().enabled = true;
                player.GetComponent<Rigidbody>().useGravity = true;

                //Disable zipline after finished using
                tag = "Untagged";

                //Finish zipline event for water
                finished_zipline = true;
            }
        }
    }

    public void Ziplining()
    {
        //Set the start time to current time
        start_time = Time.time;

        //Create a vector to determine distance travelled
        journey_length = Vector3.Distance(start_location.position, end_location.position);

        //Disable movement, climbing and gravity for player
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerClimbing>().enabled = false;
        player.GetComponent<Rigidbody>().useGravity = false;

        //Start ziplining event
        ziplining = true;
    }
}