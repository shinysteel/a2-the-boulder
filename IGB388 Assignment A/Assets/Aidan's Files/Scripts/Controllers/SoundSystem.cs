using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    public class SoundSystem
    {
        public static void PlaySound(Vector3 pos, string id)
        {
            PlaySound(pos, GeneralDatabase.Singleton.GetReference<AudioClip>(id));
        }
        public static void PlayRandomSound(Vector3 pos, params string[] ids)
        {
            int rand = Random.Range(0, ids.Length);
            PlaySound(pos, GeneralDatabase.Singleton.GetReference<AudioClip>(ids[rand]));
        }



        private static void PlaySound(Vector3 pos, AudioClip audioClip)
        {
            if (audioClip == null)
                return;
            GameObject go = new GameObject("Sound");
            go.transform.position = pos;
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            float randomPitch = Random.Range(0.88f, 1.12f);
            audioSource.pitch = randomPitch;
            audioSource.Play();
            MonoBehaviour.Destroy(go, audioClip.length);
        }
    }
}
