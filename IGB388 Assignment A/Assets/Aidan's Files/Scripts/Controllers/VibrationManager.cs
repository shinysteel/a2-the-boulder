using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    /// Manager for receiving vibration requests
    /// and simulating them on controllers. 
    public class VibrationManager : MonoBehaviour
    {
        public static VibrationManager Singleton;
        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }

        private bool[] vibrating = new bool[2] { false, false };
        private float[] timers = new float[2] { 0f, 0f };

        // Triggers a vibration for a given controller and duration.
        public void TriggerVibration(float duration, OVRInput.Controller controller)
        {
            int i = controller == OVRInput.Controller.LTouch ? 0 : 1;
            vibrating[i] = true;
            timers[i] = duration;
            OVRInput.SetControllerVibration(1f, 1f, controller);
        }

        // Disables controllers if their duration has elapsed. 
        private void UpdateController(OVRInput.Controller controller)
        {
            int i = controller == OVRInput.Controller.LTouch ? 0 : 1;
            if (!vibrating[i])
                return;
            timers[i] -= Time.deltaTime;
            if (timers[i] <= 0f)
            {
                vibrating[i] = false;
                OVRInput.SetControllerVibration(0f, 0f, controller);
            }
        }
        private void Update()
        {
            UpdateController(OVRInput.Controller.LTouch);
            UpdateController(OVRInput.Controller.RTouch);
        }
    }
}