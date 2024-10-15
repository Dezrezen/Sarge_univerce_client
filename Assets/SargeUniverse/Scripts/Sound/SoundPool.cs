using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Sound
{
    public interface ISoundPool
    {
        ISoundInstance Spawn(GameSound gameSound, float volume);
        void           Remove(ISoundInstance instance);
    }
    
    public class SoundPool : ISoundPool
    {
        private List<ISoundInstance> _poolList = new();
        
        private readonly SoundInstance _soundInstance;
        private readonly Transform     _holder;

        public SoundPool(SoundInstance soundInstance, Transform holder)
        {
            _holder        = holder;
            _soundInstance = soundInstance;
        }
        
        public ISoundInstance Spawn(GameSound gameSound, float volume)
        {
            ISoundInstance instance = Object.Instantiate(_soundInstance, _holder);
            instance.Init(this, gameSound.clip, volume);
            _poolList.Add(instance);

            return instance;
        }

        public void Remove(ISoundInstance instance)
        {
            _poolList.Remove(instance);
        }
    }
}