using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Trigger : MonoBehaviour
{
    public AudioSource sound;

    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sound.Play();
            GetComponent<Collider>().enabled = false;
        }
    }
}
