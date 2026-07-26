using UnityEngine;
using Anoa.Module;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using AudioSystem;

namespace GMTK.Enemy
{
    [System.Serializable]
    public class PathWaveEvent
    {
        [Tooltip("Delay antar wave")]
        public float fltDelay = 1f;

        [Tooltip("Gudang (PoolerContainer) musuh yang akan di-spawn (misal: Pool Musuh Biasa, Pool Musuh Elite).")]
        public PoolerContainer enemyPool;

        [Tooltip("Referensi ke component DOTweenPath di Scene yang menentukan jalur pergerakan.")]
        public DOTweenPath dotweenPathReference;

        [Tooltip("Jumlah musuh yang akan muncul secara beruntun dari event ini.")]
        public int count = 1;

        [Tooltip("Jeda waktu antar musuh jika jumlahnya (count) lebih dari 1.")]
        public float spawnInterval = 0.5f;

        [Tooltip("Jika dicentang, musuh tidak akan dinonaktifkan saat mencapai titik akhir path.")]
        public bool dontDeactivateAtEnd;

        [Tooltip("Jika dicentang, event wave ini akan dianggap sebagai awal boss dan memicu pergantian BGM ke boss.")]
        public bool triggerBossBgm;
    }

    public class EnemyWaveSpawnerPath : MonoBehaviour
    {
        [Header("Wave Timeline")]
        [Tooltip("Daftar urutan kemunculan musuh (Timeline). Akan dieksekusi dari atas ke bawah.")]
        public List<PathWaveEvent> waveTimeline = new List<PathWaveEvent>();

        [Header("BGM")]
        [Tooltip("BGM loop untuk wave minion (contoh: Level 1/2 minion theme).")]
        [SerializeField] protected SoundData minionBgmSound;
        [Tooltip("BGM loop untuk wave boss yang akan ditambahkan di atas minion BGM.")]
        [SerializeField] protected SoundData bossBgmSound;
        [Tooltip("BGM akhir setelah boss kalah untuk level non-3 (opsional).")]
        [SerializeField] protected SoundData finalMinionBgmSound;
        [Tooltip("BGM akhir setelah boss kalah untuk level 3 (opsional).")]
        [SerializeField] protected SoundData finalBossBgmSound;
        [SerializeField] protected bool isLevel3;
        [SerializeField] protected bool playMinionBgmOnStart = true;
        [SerializeField] protected float bgmFadeDuration = 1.0f;

        protected bool bossBgmTriggered;
        protected AudioSource minionBgmSource;
        protected AudioSource bossBgmSource;
        protected Coroutine minionFadeCoroutine;
        protected Coroutine bossFadeCoroutine;

        void Awake()
        {
            minionBgmSource = gameObject.AddComponent<AudioSource>();
            bossBgmSource = gameObject.AddComponent<AudioSource>();
        }

        void Start()
        {
            if (playMinionBgmOnStart)
            {
                PlayMinionBgm();
            }

            StartCoroutine(SpawnTimelineRoutine());
        }

        private IEnumerator SpawnTimelineRoutine()
        {
            foreach (PathWaveEvent waveEvent in waveTimeline)
            {
                // Tunggu sesuai jeda waktu sebelum memunculkan musuh ini
                if (waveEvent.fltDelay > 0)
                {
                    yield return new WaitForSeconds(waveEvent.fltDelay);
                }

                StartCoroutine(IESpawnEnemy(waveEvent));
            }
        }

        protected IEnumerator IESpawnEnemy(PathWaveEvent waveEvent)
        {
            // Munculkan musuh sebanyak 'count' kali
            for (int i = 0; i < waveEvent.count; i++)
            {
                SpawnEnemy(waveEvent);

                // Jeda antar musuh dalam satu event (jika ada lebih dari 1)
                if (i < waveEvent.count - 1 && waveEvent.spawnInterval > 0)
                {
                    yield return new WaitForSeconds(waveEvent.spawnInterval);
                }
            }
        }

        private void SpawnEnemy(PathWaveEvent waveEvent)
        {
            if (!bossBgmTriggered && waveEvent.triggerBossBgm)
            {
                bossBgmTriggered = true;
                PlayBossOverlayBgm();
            }

            if (waveEvent.enemyPool == null)
            {
                Debug.LogWarning("Ada Pooler yang kosong di daftar Wave Timeline!");
                return;
            }

            if (waveEvent.dotweenPathReference == null)
            {
                Debug.LogWarning("Ada DOTweenPath Reference yang kosong di daftar Wave Timeline!");
                return;
            }

            var wps = waveEvent.dotweenPathReference.wps;
            if (wps == null || wps.Count == 0)
            {
                Debug.LogWarning("DOTweenPath Reference tidak memiliki waypoints!");
                return;
            }

            // Arah basis path (relative vs absolute)
            Vector3 pathOrigin = waveEvent.dotweenPathReference.relative ? waveEvent.dotweenPathReference.transform.position : Vector3.zero;

            // Titik spawn diambil dari waypoint pertama (index 0) dari DOTweenPath
            Vector3 spawnPos = wps[0] + pathOrigin;

            // Mengambil musuh dari pooler
            GameObject enemyObj = waveEvent.enemyPool.Pop();

            if (enemyObj != null)
            {
                // Atur posisi agar muncul di titik spawn (waypoint pertama)
                enemyObj.transform.position = spawnPos;
                enemyObj.transform.rotation = transform.rotation;

                // Cari component EnemyPathController pada prefab musuh
                EnemyPathController pathCtrl = enemyObj.GetComponent<EnemyPathController>();
                if (pathCtrl != null)
                {
                    // Salin parameter gerakan dari DOTweenPath
                    pathCtrl.SetPathSettings(
                        waveEvent.dotweenPathReference.duration,
                        waveEvent.dotweenPathReference.easeType,
                        waveEvent.dotweenPathReference.pathType,
                        waveEvent.dotweenPathReference.pathMode
                    );

                    pathCtrl.SetDontDeactivate(waveEvent.dontDeactivateAtEnd);

                    // Konversi sisa waypoints (dari index 1 dst) ke array Vector2
                    if (wps.Count > 1)
                    {
                        Vector2[] path2D = new Vector2[wps.Count - 1];
                        for (int i = 1; i < wps.Count; i++)
                        {
                            path2D[i - 1] = (Vector2)(wps[i] + pathOrigin);
                        }
                        pathCtrl.SetPath(path2D);
                    }
                    else
                    {
                        // Jika hanya ada 1 waypoint, musuh hanya diam di tempat spawn
                        pathCtrl.SetPath(new Vector2[0]);
                    }
                }
            }
        }

        protected void PlayMinionBgm()
        {
            if (minionBgmSound == null)
            {
                return;
            }

            ConfigureSource(minionBgmSource, minionBgmSound);
            minionBgmSource.volume = minionBgmSound.volume;
            minionBgmSource.Play();
        }

        protected void PlayBossOverlayBgm()
        {
            if (bossBgmSound == null)
            {
                return;
            }

            ConfigureSource(bossBgmSource, bossBgmSound);
            bossBgmSource.volume = bossBgmSound.volume;
            bossBgmSource.Play();
        }

        public void OnBossDefeated()
        {
            SoundData postBossBgm = isLevel3 ? finalBossBgmSound : finalMinionBgmSound;

            if (postBossBgm != null)
            {
                CrossfadeMinionTo(postBossBgm);
            }

            FadeOutBossOverlay();
        }

        protected void CrossfadeMinionTo(SoundData targetSound)
        {
            if (targetSound == null)
            {
                return;
            }

            if (minionFadeCoroutine != null)
            {
                StopCoroutine(minionFadeCoroutine);
            }

            minionFadeCoroutine = StartCoroutine(IECrossfadeMinion(targetSound));
        }

        protected IEnumerator IECrossfadeMinion(SoundData targetSound)
        {
            float duration = Mathf.Max(0.01f, bgmFadeDuration);
            float elapsed = 0f;
            float startVolume = minionBgmSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                minionBgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            ConfigureSource(minionBgmSource, targetSound);
            minionBgmSource.volume = 0f;
            minionBgmSource.Play();

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                minionBgmSource.volume = Mathf.Lerp(0f, targetSound.volume, elapsed / duration);
                yield return null;
            }

            minionBgmSource.volume = targetSound.volume;
            minionFadeCoroutine = null;
        }

        protected void FadeOutBossOverlay()
        {
            if (!bossBgmSource.isPlaying)
            {
                return;
            }

            if (bossFadeCoroutine != null)
            {
                StopCoroutine(bossFadeCoroutine);
            }

            bossFadeCoroutine = StartCoroutine(IEFadeOutBoss());
        }

        protected IEnumerator IEFadeOutBoss()
        {
            float duration = Mathf.Max(0.01f, bgmFadeDuration);
            float elapsed = 0f;
            float startVolume = bossBgmSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                bossBgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            bossBgmSource.Stop();
            bossFadeCoroutine = null;
        }

        protected void ConfigureSource(AudioSource source, SoundData data)
        {
            source.clip = data.Clip;
            source.outputAudioMixerGroup = data.MixerGroup;
            source.loop = data.Loop;
            source.playOnAwake = false;

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

        private void OnDrawGizmosSelected()
        {
            if (waveTimeline == null) return;

            foreach (PathWaveEvent waveEvent in waveTimeline)
            {
                if (waveEvent.dotweenPathReference == null) continue;

                var wps = waveEvent.dotweenPathReference.wps;
                if (wps == null || wps.Count == 0) continue;

                Vector3 pathOrigin = waveEvent.dotweenPathReference.relative ? waveEvent.dotweenPathReference.transform.position : Vector3.zero;

                // 1. Gambar bola hijau di waypoint pertama (sebagai titik spawn baru)
                Vector3 spawnPos = wps[0] + pathOrigin;
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(spawnPos, 0.3f);

                // 2. Gambar visualisasi path menghubungkan waypoint berikutnya (Cyan)
                if (wps.Count > 1)
                {
                    Gizmos.color = Color.cyan;
                    Vector3 prevPoint = spawnPos;
                    for (int i = 1; i < wps.Count; i++)
                    {
                        Vector3 currentPoint = wps[i] + pathOrigin;
                        Gizmos.DrawWireSphere(currentPoint, 0.2f);
                        Gizmos.DrawLine(prevPoint, currentPoint);
                        prevPoint = currentPoint;
                    }
                }
            }
        }
    }
}
