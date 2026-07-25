using UnityEngine;
using Anoa.Module;
using System.Collections;
using System.Collections.Generic;

namespace GMTK.Enemy
{
    [System.Serializable]
    public class WaveEvent
    {
        [Tooltip("Waktu tunggu (dalam detik) SEBELUM musuh ini muncul. (Dihitung setelah musuh sebelumnya muncul)")]
        public float delay = 1f;

        [Tooltip("Gudang (PoolerContainer) musuh yang akan di-spawn (misal: Pool Musuh Biasa, Pool Musuh Elite).")]
        public PoolerContainer enemyPool;

        [Tooltip("Jika False, makan akan menggunakan spawn point default dari prefab")]
        public bool useSpecificSpawnPosition;

        [Tooltip("Titik awal kemunculan musuh (X, Y) di layar.")]
        public Vector2 spawnPosition;

        [Tooltip("Jika False, makan akan menggunakan target point default dari prefab")]
        public bool useTargetPoint;

        [Tooltip("Titik arah tujuan musuh (X, Y). Musuh akan terbang lurus ke arah ini.")]
        public Vector2 targetPosition;

        [Tooltip("Jumlah musuh yang akan muncul secara beruntun dari event ini.")]
        public int count = 1;

        [Tooltip("Jeda waktu antar musuh jika jumlahnya (count) lebih dari 1.")]
        public float spawnInterval = 0.5f;
    }

    public class EnemyWaveSpawnerLevel2 : MonoBehaviour
    {
        [Header("Wave Timeline")]
        [Tooltip("Daftar urutan kemunculan musuh (Timeline). Akan dieksekusi dari atas ke bawah.")]
        public List<WaveEvent> waveTimeline = new List<WaveEvent>();

        void Start()
        {
            StartCoroutine(SpawnTimelineRoutine());
        }

        private IEnumerator SpawnTimelineRoutine()
        {
            foreach (WaveEvent waveEvent in waveTimeline)
            {
                // Tunggu sesuai jeda waktu sebelum memunculkan musuh ini
                if (waveEvent.delay > 0)
                {
                    yield return new WaitForSeconds(waveEvent.delay);
                }

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
        }

        private void SpawnEnemy(WaveEvent waveEvent)
        {
            if (waveEvent.enemyPool == null)
            {
                Debug.LogWarning("Ada Pooler yang kosong di daftar Wave Timeline!");
                return;
            }

            // Gunakan kordinat spesifik jika dicentang, jika tidak gunakan posisi spawner ini.
            Vector3 spawnPos = waveEvent.useSpecificSpawnPosition ? (Vector3)waveEvent.spawnPosition : transform.position;

            // Mengambil musuh dari pooler
            GameObject enemyObj = waveEvent.enemyPool.Pop();
            
            if (enemyObj != null)
            {
                // Atur posisi agar muncul di titik spawn
                enemyObj.transform.position = spawnPos;
                enemyObj.transform.rotation = transform.rotation;

                // Jika ada target arah, berikan ke musuh
                if (waveEvent.useTargetPoint)
                {
                    EnemyLevel2Basic enemyScript = enemyObj.GetComponent<EnemyLevel2Basic>();
                    if (enemyScript != null)
                    {
                        enemyScript.SetTargetPoint(waveEvent.targetPosition);
                    }
                }
            }
        }
    }
}
