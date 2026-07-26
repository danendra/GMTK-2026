using UnityEngine;

using Anoa.Module;
using System.Collections;

namespace GMTK.Enemy
{
    public class EnemyShootingController : MonoBehaviour
    {
        [SerializeField] protected float fltInitialDelay = 0f;
        [SerializeField] protected float fltDelay = 0.2f;

        [SerializeField] protected bool isRandomize = false;
        [SerializeField] protected bool canStartShot = true;
        [SerializeField] protected Vector2 minMaxDelay;

        protected EnemyBulletSpawnerController[] arrBulletSpawners;
        protected CooldownModule cooldown;

        void Awake()
        {
            arrBulletSpawners = GetComponentsInChildren<EnemyBulletSpawnerController>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            yield return new WaitForSeconds(fltInitialDelay);

            Initialize();
        }

        void OnEnable()
        {
            Initialize();
        }

        protected void Initialize()
        {
            if (isRandomize)
            {
                cooldown = new CooldownModule(Random.Range(minMaxDelay.x, minMaxDelay.y), canStartShot);
            }
            else
            {
                cooldown = new CooldownModule(fltDelay, canStartShot);
            }      
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown != null && cooldown.IsReady)
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