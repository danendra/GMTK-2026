using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AudioSystem
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> _soundEmitterPool;
        readonly List<SoundEmitter> _activeEmitters = new();
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
                    // Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get()
        {
            return _soundEmitterPool.Get();
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
    }
}