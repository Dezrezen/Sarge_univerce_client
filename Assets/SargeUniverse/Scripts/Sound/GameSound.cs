using System;
using UnityEngine;

namespace SargeUniverse.Scripts.Sound
{
    [Serializable]
    public class GameSound
    {
        public string sound_name;
        public AudioClip clip;
        [Range(0.0f, 1.0f)]
        public float volume = 1f;

        public void UpdateNaming()
        {
            if (string.IsNullOrEmpty(sound_name) && clip != null)
            {
                sound_name = clip.name;
            }
        }
    }
}