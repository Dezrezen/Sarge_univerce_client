using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Sound
{
    public interface ISoundSystem
    {
        bool IsSoundAvailable { get; }
        bool IsMusicAvailable { get; }
        float SoundVolume { get; }
        float MusicVolume { get; }
        
        void PlaySound(string soundName);
        void PlayMusic(string musicName);
        void StopMusic();
        void DisableSounds();
        void DisableMusic();
        void EnableSound();
        void EnableMusic();
        void SetSoundVolume(float volume);
        void SetMusicVolume(float volume);
    }
    
    public class SoundSystem : MonoBehaviour, ISoundSystem
    {
        private const string SOUND_AVAILABLE_KEY = "SoundAvailable";
        private const string MUSIC_AVAILABLE_KEY = "MusicAvailable";
        
        private const string SOUND_VOLUME_KEY = "SoundVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";

        [SerializeField] private SoundListSO _soundListSO;

        private Dictionary<string, GameSound> _soundStorage;
        private ISoundPool _soundsPool;
        
        private ISoundInstance _musicInstance;
        private GameSound[] _gameSounds;

        private string _lastMusic;
        
        public bool IsSoundAvailable { get; private set; }
        public bool IsMusicAvailable { get; private set; }
        
        public float SoundVolume { get; private set; }
        public float MusicVolume { get; private set; }

        public static ISoundSystem Instance;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
            
            _soundStorage = _soundListSO.GetSounds();
            _soundsPool = new SoundPool(_soundListSO.GetSoundInstance(), transform);
            
            IsSoundAvailable = PlayerPrefs.GetInt(SOUND_AVAILABLE_KEY, 1) == 1;
            IsMusicAvailable = PlayerPrefs.GetInt(MUSIC_AVAILABLE_KEY, 1) == 1;

            SoundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 0.8f);
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.8f);
        }

        public void DisableSounds()
        {
            IsSoundAvailable = false;
            PlayerPrefs.SetInt(SOUND_AVAILABLE_KEY, 0);
            PlayerPrefs.Save();
        }

        public void DisableMusic()
        {
            IsMusicAvailable = false;
            PlayerPrefs.SetInt(MUSIC_AVAILABLE_KEY, 0);
            PlayerPrefs.Save();

            _musicInstance?.StopMusic();
        }

        public void EnableSound()
        {
            IsSoundAvailable = true;
            PlayerPrefs.SetInt(SOUND_AVAILABLE_KEY, 1);
            PlayerPrefs.Save();
        }

        public void EnableMusic()
        {
            IsMusicAvailable = true;
            PlayerPrefs.SetInt(MUSIC_AVAILABLE_KEY, 1);
            PlayerPrefs.Save();

            if (string.IsNullOrEmpty(_lastMusic) == false)
            {
                PlayMusic(_lastMusic);
            }
        }
        
        public void PlaySound(string soundName)
        {
            if (IsSoundAvailable == false)
            {
                return;
            }

            if (_soundStorage.ContainsKey(soundName) == false)
            {
                return;
            }
            
            var instance = _soundsPool.Spawn(_soundStorage[soundName], SoundVolume);
            instance.PlaySound();
        }

        public void PlaySound(string soundName, Vector3 position)
        {
            if (IsSoundAvailable == false)
            {
                return;
            }

            if (_soundStorage.ContainsKey(soundName) == false)
            {
                return;
            }
            
            var instance = _soundsPool.Spawn(_soundStorage[soundName], SoundVolume);
            instance.PlaySound(position);
        }
 
        public void PlayMusic(string musicName)
        {
            if (_soundStorage.ContainsKey(musicName) == false)
            {
                return;
            }

            /*if (string.CompareOrdinal(_lastMusic, musicName) == 0)
            {
                return;
            }*/

            _musicInstance?.StopMusic();
            _musicInstance = _soundsPool.Spawn(_soundStorage[musicName], MusicVolume);

            _lastMusic = musicName;

            if (IsMusicAvailable == false)
            {
                return;
            }
            
            _musicInstance.PlayMusic();
        }

        public void StopMusic()
        {
            _musicInstance?.StopMusic();
        }

        public void SetSoundVolume(float volume)
        {
            SoundVolume = volume;
            PlayerPrefs.SetInt(SOUND_VOLUME_KEY, 1);
            PlayerPrefs.Save();
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
            PlayerPrefs.SetInt(MUSIC_VOLUME_KEY, 1);
            PlayerPrefs.Save();
            
            _musicInstance?.ChangeVolume(volume);
        }
    }
}