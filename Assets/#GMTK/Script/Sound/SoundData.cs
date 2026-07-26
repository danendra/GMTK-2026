using UnityEngine;
using System;
using UnityEngine.Audio;

namespace AudioSystem{
    [Serializable]
    public class SoundData
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] private bool _loop;
        [SerializeField] private bool _playOnAwake;
        [SerializeField] private bool _frequentSound;

        public AudioClip Clip => _clip;
        public AudioMixerGroup MixerGroup => _mixerGroup;
        public bool Loop => _loop;
        public bool PlayOnAwake => _playOnAwake;
        public bool FrequentSound => _frequentSound;
    
        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;

        public int priority = 128;
        public float volume = 1.0f;
        public float pitch = 1.0f;
        public float panStereo;
        public float spatialBlend;
        public float reverbZoneMix = 1.0f;
        public float dopplerLevel = 1.0f;
        public float spread;

        public float minDistance = 1.0f;
        public float maxDistance = 500.0f;

        public bool ignoreListenerVolume;
        public bool ignoreListenerPause;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    }
}