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
        private void Start()
        {
            movement = GetComponent<PlayerMovement>();
            climbing = GetComponent<PlayerClimbing>();

            climbing.onClimbStart += OnClimbStart;
            climbing.onClimbEnd += OnClimbEnd;
        }

        private void OnClimbStart()
        {
            UpdateState(eState.Climbing);
        }
        private void OnClimbEnd()
        {
            UpdateState(eState.Grounded);
        }


        private void UpdateState(eState newState)
        {
            state = newState;
            movement.enabled = state == eState.Grounded;
        }
    }
}
