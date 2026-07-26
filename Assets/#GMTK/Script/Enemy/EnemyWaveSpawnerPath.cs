using UnityEngine;
using Anoa.Module;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

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

        [Tooltip("Event yang dipanggil saat wave ini mulai diproses.")]
        public UnityEvent onStart = new UnityEvent();

    }

    public class EnemyWaveSpawnerPath : MonoBehaviour
    {
        [Header("Wave Timeline")]
        [Tooltip("Daftar urutan kemunculan musuh (Timeline). Akan dieksekusi dari atas ke bawah.")]
        public List<PathWaveEvent> waveTimeline = new List<PathWaveEvent>();

        void Start()
        {
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

                waveEvent.onStart.Invoke();

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
