using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    /// General game controller for global referencing.
    public class GC : MonoBehaviour
    {
        public static GC Singleton;
        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }
    }
}
