
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : Singleton<AudioSettingsManager> {
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    public AudioSettings bgmSettings = new("bgmVol");
    [SerializeField]
    public AudioSettings sfxSettings = new("sfxVol");

    private void Update() {
        ((IUpdate)bgmSettings).Update();
        ((IUpdate)sfxSettings).Update();
    }

    private interface IUpdate
    {
        void Update();
    }

    [Serializable]
    public class AudioSettings : IUpdate {
        // MARK: public apis

        /// <summary>
        /// Set the volume of the audio
        /// </summary>
        /// <param name="volume">Volume, will be clamped to [0.001, 1.0]</param>
        public void SetVolume(float volume) {
            Volume = volume;
        }

        /// <summary>
        /// Get the volume of the audio
        /// </summary>
        /// <returns>Volume in [0.001, 1.0]</returns>
        public float GetVolume() {
            return Volume;
        }

        #region inners

        private readonly string field;

        public AudioSettings(string field) {
            this.field = field;
        }

        // For inspector
        [SerializeField, Range(0.001f, 1.0f)]
        private float Volume = 1.0f;

        void IUpdate.Update() {
            // Update from inspector
            if (volume != Volume) {
                volume = Volume;
            }
        }

        private float volume {
            get {
                AudioSettingsManager.Instance.mixer.GetFloat(field, out var value);
                return Mathf.Clamp(Mathf.Pow(10, value / 20), 0.001f, 1.0f);
            }
            set {
                value = Mathf.Log10(Mathf.Clamp(value, 0.001f, 1.0f)) * 20;
                AudioSettingsManager.Instance.mixer.SetFloat(field, value);
            }
        }
        #endregion
    }
}
