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
        public RectTransform upIndicatorRT;
        public RectTransform leftIndicatorRT;
        public RectTransform downIndicatorRT;
        public RectTransform rightIndicatorRT;

        private const float SIDEHEIGHT_WITHOUT_UPORDOWN = 450f;
        private const float SIDEHEIGHT_WITH_UPANDDOWN = 220f;
        private const float SIDEHEIGHT_WITH_UPXORDOWN = 425f; // XOR = exclusive OR. 
        private const float SIDEYOFFSET_WITH_UPANDDOWN = 0f;
        private const float SIDEYOFFSET_WITHOUT_UPANDDOWN = 0f;
        private const float SIDEYOFFSET_WITH_UP = -100f;
        private const float SIDEYOFFSET_WITH_DOWN = 100f;

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                showHazards = !showHazards;
                hazardDebugVisual.SetActive(!hazardDebugVisual.activeSelf);
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

            upIndicatorRT.gameObject.SetActive(isUpIndicatorOn ? true : false);
            downIndicatorRT.gameObject.SetActive(isDownIndicatorOn ? true : false);
            leftIndicatorRT.gameObject.SetActive(isLeftIndicatorOn ? true : false);
            rightIndicatorRT.gameObject.SetActive(isRightIndicatorOn ? true : false);

            // Adjusting side indicators to not overlap with vertical borders (height and yoffset).
            if (isUpIndicatorOn && isDownIndicatorOn)
            {
                ChangeHeightAndYOffset(leftIndicatorRT, SIDEHEIGHT_WITH_UPANDDOWN, SIDEYOFFSET_WITH_UPANDDOWN);
                ChangeHeightAndYOffset(rightIndicatorRT, SIDEHEIGHT_WITH_UPANDDOWN, SIDEYOFFSET_WITH_UPANDDOWN);
            } 
            else if (isUpIndicatorOn ^ isDownIndicatorOn) 
            {
                ChangeHeightAndYOffset(leftIndicatorRT, SIDEHEIGHT_WITH_UPXORDOWN, (isUpIndicatorOn && !isDownIndicatorOn) ? SIDEYOFFSET_WITH_UP : SIDEYOFFSET_WITH_DOWN);
                ChangeHeightAndYOffset(rightIndicatorRT, SIDEHEIGHT_WITH_UPXORDOWN, (isUpIndicatorOn && !isDownIndicatorOn) ? SIDEYOFFSET_WITH_UP : SIDEYOFFSET_WITH_DOWN);
            }
            else // Case: both and down are off. 
            {
                ChangeHeightAndYOffset(leftIndicatorRT, SIDEHEIGHT_WITHOUT_UPORDOWN, SIDEYOFFSET_WITHOUT_UPANDDOWN);
                ChangeHeightAndYOffset(rightIndicatorRT, SIDEHEIGHT_WITHOUT_UPORDOWN, SIDEYOFFSET_WITHOUT_UPANDDOWN);
            }
        }

        private void ChangeHeightAndYOffset(RectTransform rt, float newHeight, float newYOffset)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, newYOffset);
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