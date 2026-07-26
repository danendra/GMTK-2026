using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace AudioSystem
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeEmitters = new();
        readonly List<AudioSource> _dedicatedSources = new();
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;

        void Start()
        {
            InitializePool();
        }

        public SoundBuilder CreateSound()
        {
            return new SoundBuilder(this);
        }

        public bool CanPlaySound(SoundData data)
        {
            if (!data.FrequentSound) return true;

            if (FrequentSoundEmitters.Count >= maxSoundInstances && FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return _soundEmitterPool.Get();
        }

        public void PlayDedicated(SoundData data, Vector3 position, bool randomPitch = false)
        {
            if (data == null || data.Clip == null)
            {
                return;
            }

            if (data.Loop)
            {
                for (int i = 0; i < _dedicatedSources.Count; i++)
                {
                    AudioSource existing = _dedicatedSources[i];
                    if (existing != null && existing.isPlaying && existing.loop && existing.clip == data.Clip)
                    {
                        existing.transform.position = position;
                        return;
                    }
                }
            }

            GameObject go = new GameObject($"DedicatedAudio_{data.Clip.name}");
            go.transform.SetParent(transform);
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            ApplySoundData(source, data);

            if (randomPitch)
            {
                source.pitch = Mathf.Clamp(source.pitch + Random.Range(-0.05f, 0.05f), -3f, 3f);
            }

            _dedicatedSources.Add(source);
            source.Play();

            if (!source.loop)
            {
                StartCoroutine(ReleaseDedicatedWhenDone(source));
            }
        }

        public void ReturnToPool(SoundEmitter emitter)
        {
            _soundEmitterPool.Release(emitter);
        }

        void OnDestroyPoolObject(SoundEmitter emitter)
        {
            Destroy(emitter.gameObject);
        }

        void OnReturnedToPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            _activeEmitters.Remove(emitter);
        }

        void OnTakeFromPool(SoundEmitter emitter)
        {
            _activeEmitters.Add(emitter);
            emitter.gameObject.SetActive(true);
        }

        SoundEmitter CreateSoundEmitter()
        {
            var emitter = Instantiate(soundEmitterPrefab);
            emitter.gameObject.SetActive(false);
            return emitter;
        }

        void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        IEnumerator ReleaseDedicatedWhenDone(AudioSource source)
        {
            if (source == null)
            {
                yield break;
            }

            yield return new WaitWhile(() => source != null && source.isPlaying);

            if (source != null)
            {
                _dedicatedSources.Remove(source);
                Destroy(source.gameObject);
            }
        }

        static void ApplySoundData(AudioSource source, SoundData data)
        {
            source.clip = data.Clip;
            source.outputAudioMixerGroup = data.MixerGroup;
            source.loop = data.Loop;
            source.playOnAwake = data.PlayOnAwake;

            source.mute = data.mute;
            source.bypassEffects = data.bypassEffects;
            source.bypassListenerEffects = data.bypassListenerEffects;
            source.bypassReverbZones = data.bypassReverbZones;

            source.priority = data.priority;
            source.volume = data.volume;
            source.pitch = data.pitch;
            source.panStereo = data.panStereo;
            source.spatialBlend = data.spatialBlend;
            source.reverbZoneMix = data.reverbZoneMix;
            source.dopplerLevel = data.dopplerLevel;
            source.spread = data.spread;

            source.minDistance = data.minDistance;
            source.maxDistance = data.maxDistance;

            source.ignoreListenerVolume = data.ignoreListenerVolume;
            source.ignoreListenerPause = data.ignoreListenerPause;
            source.rolloffMode = data.rolloffMode;
        }
    }
}