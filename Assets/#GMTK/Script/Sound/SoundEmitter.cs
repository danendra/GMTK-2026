
using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        AudioSource audioSource;
        Coroutine playingCoroutine;

        void Awake()
        {
            audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        public void Play()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());            
        }

        public void Initialize(SoundData data)
        {
            Data = data;
            audioSource.clip = data.Clip;
            audioSource.outputAudioMixerGroup = data.MixerGroup;
            audioSource.loop = data.Loop;
            audioSource.playOnAwake = data.PlayOnAwake;
        
            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;

            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;
            
            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;

            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;
            audioSource.rolloffMode = data.rolloffMode;
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            SoundManager.Instance.ReturnToPool(this);
        }

        public void Stop(){
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            if (min > max)
            {
                (min, max) = (max, min);
            }

            audioSource.pitch = Mathf.Clamp(audioSource.pitch + Random.Range(min, max), -3f, 3f);
        }
    }
}