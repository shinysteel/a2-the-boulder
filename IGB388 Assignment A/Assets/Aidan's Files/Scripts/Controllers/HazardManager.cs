using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    public class HazardManager : MonoBehaviour
    {
        public static HazardManager Singleton;
        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }

        // Hazard Variables.
        [SerializeField]
        private Vector3 hazardVector = new Vector3(96.5f, 62.21f, 37.53f);
        private bool showHazards = false;
        public GameObject hazardDebugVisual;

        [Header("UI References")]
        public GameObject upIndicatorGO;
        public GameObject leftIndicatorGO;
        public GameObject downIndicatorGO;
        public GameObject rightIndicatorGO;

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                showHazards = !showHazards;
                hazardDebugVisual.SetActive(!hazardDebugVisual.activeSelf);
                upIndicatorGO.SetActive(showHazards);
                downIndicatorGO.SetActive(showHazards);
                leftIndicatorGO.SetActive(showHazards);
                rightIndicatorGO.SetActive(showHazards);
            }
            if (!showHazards)
                return;

            HandleIndicators();
        }

        // Handles all indicator display logic.
        private void HandleIndicators()
        {
            Vector3 playerCameraPos = VRTracker.Singleton.Headset.transform.position;
            Vector3 playerCameraDir = VRTracker.Singleton.Headset.transform.forward;
            Vector3 playerCameraToHazardDir = ((Vector3)hazardVector - playerCameraPos).normalized;

            float xAngleDif = GetAxisAngleDifference(playerCameraDir, playerCameraToHazardDir, eAxis.X, false);
            float yAngleDif = GetAxisAngleDifference(playerCameraDir, playerCameraToHazardDir, eAxis.Y, false);

            float minDif = 30f;
            bool isUpIndicatorOn = yAngleDif <= -minDif;
            bool isDownIndicatorOn = yAngleDif >= minDif;
            bool isLeftIndicatorOn = xAngleDif >= minDif;
            bool isRightIndicatorOn = xAngleDif <= -minDif;

            upIndicatorGO.SetActive(isUpIndicatorOn ? true : false);
            downIndicatorGO.SetActive(isDownIndicatorOn ? true : false);
            leftIndicatorGO.SetActive(isLeftIndicatorOn ? true : false);
            rightIndicatorGO.SetActive(isRightIndicatorOn ? true : false);
        }

        private enum eAxis { X, Y };
        private float GetAxisAngleDifference(Vector3 v1, Vector3 v2, eAxis axis, bool useAbsoluteValue) // From v1 to v2.
        {
            if (axis == eAxis.X)
                v1 = new Vector3(v1.x, v2.y, v1.z);
            else if (axis == eAxis.Y)
                v1 = new Vector3(v2.x, v1.y, v1.z);
            float angle = Vector3.Angle(v1, v2);
            if (!useAbsoluteValue)
            {
                Vector3 cross = Vector3.Cross(v1, v2);
                if (cross.y >= 0)
                    angle = -angle;
            }
            return angle;
        }
    }
}