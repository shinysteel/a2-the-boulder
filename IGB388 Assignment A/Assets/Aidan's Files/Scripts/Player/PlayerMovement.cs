using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Assignment
{
    /// Manager that maintains movement system 
    /// of the player. 
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody rb;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private float movespeed = 1.5f;
        private void Update()
        {
            HandleMovement();
        }

        // Handles movement requests.
        private void HandleMovement()
        {
            Vector2 directionalInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch).normalized;
            Vector3 directionalVector = GetDirectionalVector(directionalInput);
            rb.MovePosition(transform.position + directionalVector * movespeed * Time.deltaTime);
        }

        // Given (x,y) directional input, returns a matching directional vector 
        // in 3D space relative to the player's facing direction. 
        private Vector3 GetDirectionalVector(Vector2 directionalInput)
        {
            GameObject headset = VRTracker.Singleton.Headset;
            Vector3 camF = headset.transform.forward;
            Vector3 camR = headset.transform.right;
            camF.y = camR.y = 0f;
            camF = camF.normalized;
            camR = camR.normalized;
            return (camF * directionalInput.y + camR * directionalInput.x);
        }
    }
}
