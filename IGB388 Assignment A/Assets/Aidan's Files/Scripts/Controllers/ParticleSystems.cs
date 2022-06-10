using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    public class ParticleSystems : MonoBehaviour
    {
        public static ParticleSystems Singleton;
        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }

        public GameObject dustPS; // 0

        public void SpawnParticle(int id, Vector3 pos)
        {
            GameObject particlePrefab = null;
            switch (id)
            {
                case 0:
                    particlePrefab = dustPS;
                    break;
            }

            if (particlePrefab == null)
                return;
            GameObject particleGO = Instantiate(particlePrefab, pos, Quaternion.identity);
            particleGO.GetComponent<ParticleSystem>().Play();
            Destroy(particleGO, particleGO.GetComponent<ParticleSystem>().main.duration + particleGO.GetComponent<ParticleSystem>().main.startLifetime.constantMax);
        }
    }
}
