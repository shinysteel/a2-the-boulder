using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assignment
{
    public class Player : MonoBehaviour
    {
        private enum eState { Grounded, Climbing }
        [SerializeField] private eState state = eState.Grounded;

        private PlayerMovement movement;
        private PlayerClimbing climbing;

        //Make a boolean ground check for falling
        bool is_grounded = false;
        bool was_grounded;
        bool was_falling;
        float start_of_fall;
        bool is_falling { get { return (!is_grounded && GetComponent<Rigidbody>().velocity.y < 0); } }
        public float minimum_fall = 2f;

        //Health variables
        public float max_health = 50f;
        float current_health;
        public float health_regen = 1f;
        bool regen = false;

        //Respawn components for handling teleporting and saving checkpoints
        public Transform respawn_location;
        bool respawning = false;

        //HUD color handling
        public Color fade_to_black;
        public Color start;
        public Color end;
        public Color red_outline;

        //Boolean check for walking
        private bool is_walking;
        private string foot = "left";
        public AudioSource left_foot;
        public AudioSource right_foot;
        public AudioSource land;

        void Start()
        {
            movement = GetComponent<PlayerMovement>();
            climbing = GetComponent<PlayerClimbing>();

            climbing.onClimbStart += OnClimbStart;
            climbing.onClimbEnd += OnClimbEnd;

            current_health = max_health;
        }

        void Update()
        {
            CheckGround();

            if (!was_falling && is_falling)
            {
                start_of_fall = transform.position.y;
            }

            if (!was_grounded && is_grounded)
            {
                TakeDamage();

                if (current_health <= 0 && !respawning)
                {
                    StartCoroutine(Respawn(2));
                }
            }

            //Checks last frame if player was in the air
            was_grounded = is_grounded;
            was_falling = is_falling;

            if (!regen)
            {
                StartCoroutine(RegenHealth());
            }

            //Adjust the alpha of red outline based on missing health
            if (current_health >= 0)
            {
                red_outline.a = (max_health - current_health) / max_health;
            }

            CheckWalking();
        }

        void OnClimbStart()
        {
            UpdateState(eState.Climbing);
        }

        void OnClimbEnd()
        {
            UpdateState(eState.Grounded);
        }

        void UpdateState(eState newState)
        {
            state = newState;
            movement.enabled = state == eState.Grounded;
        }

        //Raycast for player touching the ground
        void CheckGround()
        {
            is_grounded = Physics.Raycast(transform.position + Vector3.up, -Vector3.up, 1.01f);
        }

        //Deal damage based on distance fallen
        void TakeDamage()
        {
            land.Play();

            float fall_distance = start_of_fall - transform.position.y;

            if (fall_distance > minimum_fall)
            {
                current_health -= fall_distance;
            }
        }

        IEnumerator RegenHealth()
        {
            regen = true;

            yield return new WaitForSeconds(1f);

            current_health += health_regen;

            regen = false;
        }

        public IEnumerator Respawn(float duration)
        {
            respawning = true;

            //Disable movement while teleporting
            movement.enabled = false;
            climbing.enabled = false;

            //Fade to black
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float normalized_time = t / duration;
                fade_to_black = Color.Lerp(start, end, normalized_time);

                yield return new WaitForSeconds(duration);
            }

            fade_to_black = end;

            //Restore all health to the player
            current_health = max_health;

            //Teleport player to the save points throughout the map
            transform.position = respawn_location.position;

            //Fade out of black
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float normalized_time = t / duration;
                fade_to_black = Color.Lerp(end, start, normalized_time);

                yield return new WaitForSeconds(duration);
            }

            //Re-enable movement after teleporting
            movement.enabled = true;
            climbing.enabled = true;

            respawning = false;
        }

        void CheckWalking()
        {
            if (GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                StartCoroutine(WalkingCycle());
            }
        }

        IEnumerator WalkingCycle()
        {
            if (foot == "left" && is_walking)
            {
                is_walking = false;
                left_foot.Play();
                yield return new WaitForSeconds(0.75f);
                foot = "right";
                is_walking = true;
            }

            else if (foot == "right" && is_walking)
            {
                is_walking = false;
                right_foot.Play();
                yield return new WaitForSeconds(0.75f);
                foot = "left";
                is_walking = true;
            }
        }
    }
}
