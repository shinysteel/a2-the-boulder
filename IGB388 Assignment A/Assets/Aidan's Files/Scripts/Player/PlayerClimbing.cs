using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    public class PlayerClimbing : MonoBehaviour
    {
        public PlayerClimbingHand leftClimbingHand;
        public PlayerClimbingHand rightClimbingHand;
        private Rigidbody rb;
        public bool debug = false;

        // Subscription to events from both hands (onGrab_ClimbObject, onRelease_ClimbObject).
        private void Start()
        {
            leftClimbingHand.onGrab_ClimbObject += OnGrabClimbObject;
            leftClimbingHand.onRelease_ClimbObject += OnReleaseClimbObject;
            rightClimbingHand.onGrab_ClimbObject += OnGrabClimbObject;
            rightClimbingHand.onRelease_ClimbObject += OnReleaseClimbObject;

            rb = GetComponent<Rigidbody>();
            leftClimbingHand.debug = rightClimbingHand.debug = debug;
        }

        // Capturing and storing events for logic. 
        public enum eHandType { Left, Right }
        public bool[] handStates = { false, false };                      // Reflects the state of holding a climb object or not.
        public GameObject[] handGrabbedClimbObjects = { null, null };     // Reflects grabbed climb objects.
        private void OnGrabClimbObject(eHandType handType, GameObject climbObject)
        {
            int index = (int)handType;
            handStates[index] = true;
            handGrabbedClimbObjects[index] = climbObject;
            VibrationManager.Singleton.TriggerVibration(0.1f, index == 0
                ? OVRInput.Controller.LTouch
                : OVRInput.Controller.RTouch);
        }
        private void OnReleaseClimbObject(eHandType handType)
        {
            int index = (int)handType;
            handStates[index] = false;
            handGrabbedClimbObjects[index] = null;
        }

        // IsClimbing boolean maintainment - reflects climb-state of player. 
        [SerializeField]
        private bool isClimbing;
        public Action onClimbStart;
        public Action onClimbEnd;
        private void Update()
        {
            if (!isClimbing && CheckForAtLeastOne(handStates, true))
            {
                isClimbing = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                onClimbStart();
            }
            if (isClimbing && CheckForAll(handStates, false))
            {
                isClimbing = false;
                rb.useGravity = true;
                onClimbEnd();
            }

            // Proceed only if climbing.
            if (!isClimbing)
                return;
            for (int i = 0; i < 2; i++)
            {
                if (!handStates[i])
                    continue;
                Vector3 velocity = i == 0
                    ? OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)
                    : OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
                rb.MovePosition(transform.position - velocity * Time.deltaTime);
            }
        }



        // Generic checkers used for logic with hand arrays. 
        private bool CheckForAtLeastOne<T>(T[] values, T target)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (target.Equals(values[i]))
                    return true;
            }
            return false;
        }
        private bool CheckForAll<T>(T[] values, T target)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!target.Equals(values[i]))
                    return false;
            }
            return true;
        }
    }
}
