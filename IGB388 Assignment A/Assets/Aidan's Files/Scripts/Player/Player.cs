using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //Respawn locations
        public Transform respawn_location;
        public Transform start_location;

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

                if (current_health <= 0)
                {
                    Respawn();
                }
            }

            //Checks last frame if player was in the air
            was_grounded = is_grounded;
            was_falling = is_falling;

            if (!regen)
            {
                StartCoroutine(RegenHealth());
            }
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

        void Respawn()
        {
            //Restore all health to the player
            current_health = max_health;

            //Teleport player to the save points throughout the map
            transform.position = respawn_location.position;
        }
    }
}
