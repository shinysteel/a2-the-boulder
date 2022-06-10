using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment
{
    public class GeneralDatabase
    {
        private static GeneralDatabase singleton;
        public static GeneralDatabase Singleton
        {
            get
            {
                if (singleton == null)
                    singleton = new GeneralDatabase();
                return singleton;
            }
        }
        private Hashtable database;
        private GeneralDatabase()
        {
            database = new Hashtable();

            AddDirectoryToDatabase<AudioClip>("Universal Sound FX/HUMAN/Footsteps/Trainers_Snow_Compact_Walk_Slow", "sound");
            AddDirectoryToDatabase<AudioClip>("Universal Sound FX/HUMAN/Footsteps/Trainers_Gravel_Compact_Run", "sound");
        }

        public T GetReference<T>(string id)
        {
            return (T)database[id];
        }
        private void AddDirectoryToDatabase<T>(string path, string idPrefix) where T : Object
        {
            T[] assets = Resources.LoadAll<T>(path);
            for (int i = 0; i < assets.Length; i++)
            {
                database.Add($"{idPrefix}:{assets[i].name}", assets[i]);
            }
        }
    }
}
