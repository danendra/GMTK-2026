using UnityEngine;

using Anoa.Module;

namespace GMTK.Enemy
{
    public class EnemyShootingController : MonoBehaviour
    {
        [SerializeField] protected float fltDelay = 0.2f;

        protected EnemyBulletSpawnerController[] arrBulletSpawners;
        protected CooldownModule cooldown;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cooldown = new CooldownModule(fltDelay, true);
            arrBulletSpawners = GetComponentsInChildren<EnemyBulletSpawnerController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown.IsReady)
            {
                SpawnBullet();

                cooldown.Use();
            }
        }

        public void SpawnBullet()
        {
            for (int i = 0; i < arrBulletSpawners.Length; i++)
            {
                arrBulletSpawners[i].Spawn();
            }
        }
    }
}