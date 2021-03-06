using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assignment
{
    /// Memory and retrieval script relating to all climb
    /// objects a singular hand collides with.
    public class PlayerClimbingHand : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> memory = new List<GameObject>();
        private const string CLIMB_OBJECT_TAG = "ClimbPoint";
        public GameObject markerGO;

        private float stamina = 100f;
        private const float MAX_STAMINA = 100f;
        private const float STAMINA_COST_TO_GRAB = 10f;
        private const float STAMINA_DEPLETION_RATE = 1.5f; // /s
        private const float STAMINA_REGENERATION_RATE = 15f;
        //public RectTransform staminaFillOriginRT;

        public Slider stamina_slider;
        public Image stamina_bar;
        public bool getting_tired = false;
        public AudioSource tired;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == CLIMB_OBJECT_TAG)
                memory.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (other.gameObject != memory[i])
                    continue;
                memory.RemoveAt(i);
                return;
            }
        }

        // Fires respective events for when the player grabs and releases climb objects.
        public PlayerClimbing.eHandType handType;
        public Action<PlayerClimbing.eHandType, GameObject> onGrab_ClimbObject;
        public Action<PlayerClimbing.eHandType> onRelease_ClimbObject;
        private bool isGrabbingClimbObject = false;
        private bool playedParticle = false;

        void Update()
        {
            bool isHandTriggerPressed = handType == PlayerClimbing.eHandType.Left
                ? OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)
                : OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
            bool isHandTriggerReleased = handType == PlayerClimbing.eHandType.Left
                ? OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger)
                : OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger);

            // Position marker.
            GameObject closestClimbObject = GetClosestClimbObject();
            Vector3 dustPos = Vector3.zero;
            if (closestClimbObject == null || isGrabbingClimbObject)
            {
                markerGO.SetActive(false);
                markerGO.GetComponent<ParticleSystem>().Stop();
            }
            else if (!isGrabbingClimbObject)
            {
                Vector3 dir = (closestClimbObject.transform.position - transform.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dir, out hit))
                {
                    markerGO.transform.position = hit.point;
                    markerGO.transform.LookAt(transform.position);
                    markerGO.transform.Translate(0f, 0f, 0.275f);
                    dustPos = markerGO.transform.position;
                    markerGO.SetActive(true);
                }
                if (!playedParticle)
                {
                    markerGO.GetComponent<ParticleSystem>().Play();
                    playedParticle = true;
                }
            }

            // If the hand has grabbed a climb object, fire an onGrab event. 
            if (!isGrabbingClimbObject && isHandTriggerPressed && memory.Count > 0 && stamina >= STAMINA_COST_TO_GRAB)
            {
                isGrabbingClimbObject = true;
                stamina -= STAMINA_COST_TO_GRAB;
                onGrab_ClimbObject(handType, closestClimbObject);
                ParticleSystems.Singleton.SpawnParticle(0, dustPos);
            }
            // If the hand has released a climb object, fire an onRelease event. 
            if (isGrabbingClimbObject && isHandTriggerReleased)
            {
                isGrabbingClimbObject = false;
                onRelease_ClimbObject(handType);
                playedParticle = false;
            }

            HandleStamina();
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag != "Zipline")
                return;
            //These variables should be defined at top in some way
            bool isHandTriggerPressed = handType == PlayerClimbing.eHandType.Left
                ? OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)
                : OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
            if (isHandTriggerPressed)
            {
                //Starts the ziplining action
                GC.Singleton.Zipline.GetComponent<Zipline>().Ziplining();
            }
        }

        // Iterates through memory and returns the closest
        // climb object to the hand.
        GameObject GetClosestClimbObject()
        {
            if (memory.Count <= 0)
                return null;
            int closestIndex = 0;
            float closestDistance = Vector3.Distance(transform.position, memory[0].transform.position);
            for (int i = 1; i < memory.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, memory[i].transform.position);
                if (distance > closestDistance)
                    continue;
                closestIndex = i;
                closestDistance = distance;
            }
            return memory[closestIndex];
        }

        private void HandleStamina()
        {
            // Stamina handling.
            if (stamina <= 0f)
            {
                stamina = 0f;
                isGrabbingClimbObject = false;
                onRelease_ClimbObject(handType);
            }

            if (isGrabbingClimbObject)
            {
                stamina -= STAMINA_DEPLETION_RATE * Time.deltaTime;
            }

            else
            {
                stamina = Mathf.Clamp(stamina + STAMINA_REGENERATION_RATE * Time.deltaTime, stamina, MAX_STAMINA);
            }

            stamina_slider.GetComponent<Slider>().value = stamina;

            // Handle colour visuals of grabbing
            if (stamina < MAX_STAMINA * 0.25)
            {
                stamina_bar.color = Color.red;

                if (!getting_tired)
                {
                    tired.Play();
                    getting_tired = true;
                }
            }

            else if (stamina < MAX_STAMINA * 0.6)
            {
                stamina_bar.color = Color.yellow;
                getting_tired = false;
            }

            else
            {
                stamina_bar.color = Color.green;
            }

            
            // Stamina visuals.
            //float staminaRatio = stamina / MAX_STAMINA;
            //staminaFillOriginRT.localScale = new Vector3(staminaFillOriginRT.localScale.x + 0.1f * (staminaRatio - staminaFillOriginRT.localScale.x), 1f, 1f);
        }


        [Header("Debug")]
        public bool debug;
        public TextMesh counterText;

        private void Start()
        {
            // Set max stamina to slider
            stamina_slider.GetComponent<Slider>().maxValue = MAX_STAMINA;

            // Set current value of slider to stamina value
            stamina_slider.GetComponent<Slider>().value = stamina;

            // Set current value of slider to stamina value
            stamina_bar.color = Color.green;

            counterText.gameObject.SetActive(debug);
        }
        private void FixedUpdate()
        {
            if (!debug)
                return;
            counterText.text = memory.Count.ToString();
        }
    }
}
