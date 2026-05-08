using UnityEngine;

namespace DeathCloud.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private void Awake()
        {
            // Patrón Singleton para asegurar que solo exista un AudioManager
            if (Instance == null)
            {
                Instance = this;
                // Hace que este objeto persista al cambiar entre la "Escena Menú" y la "Escena Juego"
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                // Si ya existe un AudioManager, destruimos el nuevo para evitar duplicados
                Destroy(gameObject); 
            }
        }

        /// <summary>
        /// Modifica el volumen de la música.
        /// </summary>
        /// <param name="volume">Volumen deseado (0.0 a 1.0).</param>
        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// Modifica el volumen de los efectos de sonido (SFX).
        /// </summary>
        /// <param name="volume">Volumen deseado (0.0 a 1.0).</param>
        public void SetSFXVolume(float volume)
        {
            if (_sfxSource != null)
            {
                _sfxSource.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// Silencia o reactiva todos los audios del juego.
        /// </summary>
        /// <param name="mute">True para silenciar, False para activar el sonido.</param>
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
