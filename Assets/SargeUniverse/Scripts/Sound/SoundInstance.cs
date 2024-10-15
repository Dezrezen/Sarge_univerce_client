using System;
using UnityEngine;

namespace SargeUniverse.Scripts.Sound
{
    public interface ISoundInstance : IDisposable
    {
        void Init(SoundPool pool, AudioClip clip, float volume);
        void PlaySound();
        void PlaySound(Vector3 position);
        void StopSound();
        void PlayMusic();
        void StopMusic();
        void ChangeVolume(float volume);
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class SoundInstance : MonoBehaviour, ISoundInstance
    {
        [SerializeField] private AudioSource _audioPlayer;

        private SoundPool _pool;
        private AudioClip _clip;

        private void Awake()
        {
            _audioPlayer.GetComponent<AudioSource>();
        }

        public void Init(SoundPool pool, AudioClip clip, float volume)
        {
            _pool = pool;
            _clip = clip;
            _audioPlayer.volume = volume;
        }

        public void PlaySound()
        {
            _audioPlayer.clip = _clip;
            _audioPlayer.Play();
            Invoke(nameof(Dispose), _clip.length);
        }
        
        public void PlaySound(Vector3 position)
        {
            transform.position = position;
            PlaySound();
        }

        public void PlayMusic()
        {
            _audioPlayer.clip = _clip;
            _audioPlayer.loop = true;
            _audioPlayer.Play();
        }

        public void StopSound()
        {
            _audioPlayer?.Stop();
            Dispose();
        }

        public void StopMusic()
        {
            if (_audioPlayer && _audioPlayer.clip != null)
            {
                _audioPlayer.Stop();
                Dispose();
            }
        }

        public void Dispose()
        {
            _pool.Remove(this);
            Destroy(gameObject);
        }

        public void ChangeVolume(float volume)
        {
            if (_audioPlayer && _audioPlayer.clip != null)
            {
                _audioPlayer.volume = volume;
            }
        }
    }
}