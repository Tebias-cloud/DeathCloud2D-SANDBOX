using UnityEngine;

namespace DeathCloud.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AudioManager (Auto-Generated)");
                    _instance = go.AddComponent<AudioManager>();
                }
                return _instance;
            }
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Settings")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                SetupSources();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void SetupSources()
        {
            // Si no las has asignado en el Inspector, las creamos nosotros
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource == null || clip == null) return;
            
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _masterVolume; // Se ajustará en UpdateVolumes
            _musicSource.Play();
            UpdateVolumes();
        }

        public void StopMusic()
        {
            if (_musicSource != null) _musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = Mathf.Clamp01(volume) * _masterVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (_sfxSource != null)
            {
                _sfxSource.volume = Mathf.Clamp01(volume) * _masterVolume;
            }
        }

        private void UpdateVolumes()
        {
            // Forzar actualización de volúmenes si fuera necesario
            if (_musicSource != null) _musicSource.volume *= _masterVolume;
            if (_sfxSource != null) _sfxSource.volume *= _masterVolume;
        }

        public void MuteAll(bool mute)
        {
            if (_musicSource != null) _musicSource.mute = mute;
            if (_sfxSource != null) _sfxSource.mute = mute;
        }

        /// <summary>
        /// Reproduce un efecto de sonido usando PlayOneShot (permite superponer múltiples sonidos).
        /// </summary>
        /// <param name="clip">El AudioClip de efecto a reproducir.</param>
        public void PlaySFX(AudioClip clip)
        {
            if (_sfxSource != null && clip != null)
            {
                _sfxSource.PlayOneShot(clip);
            }
        }
    }
}
