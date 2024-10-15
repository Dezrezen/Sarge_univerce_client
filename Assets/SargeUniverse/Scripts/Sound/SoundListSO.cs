using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SargeUniverse.Scripts.Sound
{
    [CreateAssetMenu(fileName = "SoundListSO", menuName = "Create/Data/SoundListSO")]
    public class SoundListSO : ScriptableObject
    {
        [SerializeField] public SoundInstance _sound_instance_prefab;
        [SerializeField] public GameSound[] _sounds;

        private void OnValidate()
        {
            foreach (GameSound sound in _sounds)
            {
                sound.UpdateNaming();
            }
        }

        public SoundInstance GetSoundInstance()
        {
            return _sound_instance_prefab;
        }

        public Dictionary<string, GameSound> GetSounds()
        {
            return _sounds.ToDictionary(sound => sound.sound_name, sound => sound);
        }
    }
}