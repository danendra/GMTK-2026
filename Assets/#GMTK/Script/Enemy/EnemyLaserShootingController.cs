using UnityEngine;
using UnityEngine.Events;

using Anoa.Module;

namespace GMTK.Enemy
{
    public class EnemyLaserShootingController : MonoBehaviour
    {
        [SerializeField] protected float fltDelay = 1.0f;
        [SerializeField] protected float fltDurationFire = 2.0f;
        [SerializeField] protected float fltDurationTelegraph = 1.0f;
        [SerializeField] protected bool isDrop = false;
        [SerializeField] protected bool canShootStart = false;

        [SerializeField] protected UnityEvent onLaserStart;
        [SerializeField] protected UnityEvent onLaserStop;

        protected EnemyLaserSpawnerController[] arrLaserSpawners;
        protected CooldownModule cooldown;
        protected CooldownModule cooldownDuration;
        protected Transform transParent;
        protected bool isShooting = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cooldown = new CooldownModule(fltDelay, canShootStart);
            cooldownDuration = new CooldownModule(fltDurationFire + fltDurationTelegraph, false);
            arrLaserSpawners = GetComponentsInChildren<EnemyLaserSpawnerController>();

            transParent = transform.parent;
        }

        // Update is called once per frame
        void Update()
        {
            if (isShooting)
            {
                if (cooldownDuration.IsReady)
                {
                    isShooting = false;
                    
                    onLaserStop.Invoke();
                    cooldown.Use();
                }
            }
            else
            {
                if (cooldown.IsReady)
                {
                    SpawnLaser();

                    cooldownDuration.Use();

                    isShooting = true;
                }
            }
        }

        public void SpawnLaser()
        {
            onLaserStart.Invoke();

            if (isDrop)
            {
                transform.position = transParent.position;

                transform.parent = null;
            }

            for (int i = 0; i < arrLaserSpawners.Length; i++)
            {
                arrLaserSpawners[i].Spawn(fltDurationFire, fltDurationTelegraph);
            }
        }
    }
}