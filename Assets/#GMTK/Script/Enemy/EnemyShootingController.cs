using UnityEngine;

using Anoa.Module;

namespace GMTK.Enemy
{
    public class EnemyShootingController : MonoBehaviour
    {
        [SerializeField] protected float fltDelay = 0.2f;

        [SerializeField] protected bool isRandomize = false;
        [SerializeField] protected Vector2 minMaxDelay;

        protected EnemyBulletSpawnerController[] arrBulletSpawners;
        protected CooldownModule cooldown;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (isRandomize)
            {
                cooldown = new CooldownModule(Random.Range(minMaxDelay.x, minMaxDelay.y), false);
            }
            else
            {
                cooldown = new CooldownModule(fltDelay, false);
            }

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

            if (isRandomize)
            {
                cooldown.Update(Random.Range(minMaxDelay.x, minMaxDelay.y));
            }
        }
    }
}