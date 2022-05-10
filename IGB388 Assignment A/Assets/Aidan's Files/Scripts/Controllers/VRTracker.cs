using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    /// Stores VR object / component values such as 
    /// player headset transform, controller transforms, etc 
    /// to be readily pulled as references.
    public class VRTracker : MonoBehaviour
    {
        public static VRTracker Singleton;
        public void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }

        public GameObject Headset;
    }
}
