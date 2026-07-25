using System.Collections;
using Anoa.Module;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Enemy
{
    public enum EnemyBulletTypeEnum
    {
        One = 100,
        Three = 101,
        ExplosiveOne = 200,
        ExplosiveThree = 201,
        SuperExplosiveOne = 300,
        SuperExplosiveThree = 301,
    }

    public enum EnemyLaserTypeEnum
    {
        One = 100,
        Two = 200,
        TwoVShape = 300,
    }

    public class EnemyBulletLaserType : MonoBehaviour
    {
        [SerializeField] protected GameObject playerObject;
        [Header("Bullet")]
        [SerializeField] protected bool isBulletTargetToPlayer = false;
        [SerializeField] protected EnemyBulletTypeEnum enemyBulletTypeEnum;
        [SerializeField] protected float bulletFltDelay = 0.1f;
        [SerializeField] protected GameObject spreadBulletObject;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerT;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerR;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerB;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerL;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerTR;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerRB;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerBL;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerLT;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerT_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerT_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerR_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerR_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerB_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerB_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerL_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerL_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerTR_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerTR_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerRB_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerRB_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerBL_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerBL_R;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerLT_L;
        [SerializeField] protected EnemyBulletSpawnerController bulletSpawnerControllerLT_R;
        [Header("Laser")]
        [SerializeField] protected bool isLaserTargetToPlayer = false;
        [SerializeField] protected EnemyLaserTypeEnum enemyLaserTypeEnum;
        [SerializeField] protected float laserFacingSmoothTime = 0.3f;
        [SerializeField] protected float laserFltDelay = 5f;
        [SerializeField] protected GameObject spreadLaserObject;
        [SerializeField] protected EnemyLaserSpawnerController laserSpawnerControllerB;
        [SerializeField] protected EnemyLaserSpawnerController laserSpawnerControllerB_L;
        [SerializeField] protected EnemyLaserSpawnerController laserSpawnerControllerB_R;
        [SerializeField] protected EnemyLaserSpawnerController laserSpawnerControllerB_LD;
        [SerializeField] protected EnemyLaserSpawnerController laserSpawnerControllerB_RD;
        [SerializeField] protected UnityEvent<GameObject> onHitWithLaser;

        protected CooldownModule bulletCooldown;
        protected CooldownModule laserCooldown;
        protected float currentLaserAngleVelocity; 
        protected bool isLaserFiring = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            bulletCooldown = new CooldownModule(bulletFltDelay, true);
            laserCooldown = new CooldownModule(laserFltDelay, false);
        }

        // Update is called once per frame
        void Update()
        {
            if (isBulletTargetToPlayer) UpdateFacingBullet();
            if (isLaserTargetToPlayer) UpdateFacingLaser();
            if (laserCooldown.IsReady)
            {
                if (isLaserFiring) return;
                isLaserFiring = true;
                switch (enemyLaserTypeEnum)
                {
                    case EnemyLaserTypeEnum.One:
                        laserSpawnerControllerB.Spawn();
                        break;
                    case EnemyLaserTypeEnum.Two:
                        laserSpawnerControllerB_L.Spawn();
                        laserSpawnerControllerB_R.Spawn();
                        break;
                    case EnemyLaserTypeEnum.TwoVShape:
                        laserSpawnerControllerB_LD.Spawn();
                        laserSpawnerControllerB_RD.Spawn();
                        break;
                }
                IEnumerator FireLaserCoroutine()
                {
                    yield return new WaitForSeconds(3f);
                    isLaserFiring = false;
                    laserCooldown.Use();
                }
                StartCoroutine(FireLaserCoroutine());
            } else if (bulletCooldown.IsReady)
            {
                switch (enemyBulletTypeEnum)
                {
                    case EnemyBulletTypeEnum.One:
                        bulletSpawnerControllerB.Spawn();
                        break;
                    case EnemyBulletTypeEnum.Three:
                        bulletSpawnerControllerB.Spawn();
                        bulletSpawnerControllerB_L.Spawn();
                        bulletSpawnerControllerB_R.Spawn();
                        break;
                    case EnemyBulletTypeEnum.ExplosiveOne:
                        bulletSpawnerControllerT.Spawn();
                        bulletSpawnerControllerR.Spawn();
                        bulletSpawnerControllerB.Spawn();
                        bulletSpawnerControllerL.Spawn();
                        break;
                    case EnemyBulletTypeEnum.ExplosiveThree:
                        bulletSpawnerControllerT.Spawn();
                        bulletSpawnerControllerT_L.Spawn();
                        bulletSpawnerControllerT_R.Spawn();
                        bulletSpawnerControllerR.Spawn();
                        bulletSpawnerControllerR_L.Spawn();
                        bulletSpawnerControllerR_R.Spawn();
                        bulletSpawnerControllerB.Spawn();
                        bulletSpawnerControllerB_L.Spawn();
                        bulletSpawnerControllerB_R.Spawn();
                        bulletSpawnerControllerL.Spawn();
                        bulletSpawnerControllerL_L.Spawn();
                        bulletSpawnerControllerL_R.Spawn();
                        break;
                    case EnemyBulletTypeEnum.SuperExplosiveOne:
                        bulletSpawnerControllerT.Spawn();
                        bulletSpawnerControllerR.Spawn();
                        bulletSpawnerControllerB.Spawn();
                        bulletSpawnerControllerL.Spawn();
                        bulletSpawnerControllerTR.Spawn();
                        bulletSpawnerControllerRB.Spawn();
                        bulletSpawnerControllerBL.Spawn();
                        bulletSpawnerControllerLT.Spawn();
                        break;
                    case EnemyBulletTypeEnum.SuperExplosiveThree:
                        bulletSpawnerControllerT.Spawn();
                        bulletSpawnerControllerR.Spawn();
                        bulletSpawnerControllerB.Spawn();
                        bulletSpawnerControllerL.Spawn();
                        bulletSpawnerControllerTR.Spawn();
                        bulletSpawnerControllerRB.Spawn();
                        bulletSpawnerControllerBL.Spawn();
                        bulletSpawnerControllerLT.Spawn();
                        bulletSpawnerControllerT_L.Spawn();
                        bulletSpawnerControllerT_R.Spawn();
                        bulletSpawnerControllerR_L.Spawn();
                        bulletSpawnerControllerR_R.Spawn();
                        bulletSpawnerControllerB_L.Spawn();
                        bulletSpawnerControllerB_R.Spawn();
                        bulletSpawnerControllerL_L.Spawn();
                        bulletSpawnerControllerL_R.Spawn();
                        bulletSpawnerControllerTR_L.Spawn();
                        bulletSpawnerControllerTR_R.Spawn();
                        bulletSpawnerControllerRB_L.Spawn();
                        bulletSpawnerControllerRB_R.Spawn();
                        bulletSpawnerControllerBL_L.Spawn();
                        bulletSpawnerControllerBL_R.Spawn();
                        bulletSpawnerControllerLT_L.Spawn();
                        bulletSpawnerControllerLT_R.Spawn();
                        break;
                }
                bulletCooldown.Use();
            }
        }

        private void UpdateFacingBullet()
        {
            if (playerObject == null || spreadBulletObject == null) return;

            Vector2 direction = playerObject.transform.position - spreadBulletObject.transform.position;
            if (direction.sqrMagnitude < 0.0001f) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            spreadBulletObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void UpdateFacingLaser()
        {
            if (playerObject == null || spreadLaserObject == null) return;

            Vector2 direction = playerObject.transform.position - spreadLaserObject.transform.position;
            if (direction.sqrMagnitude < 0.0001f) return;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
            float currentAngle = spreadLaserObject.transform.eulerAngles.z;

            float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentLaserAngleVelocity, laserFacingSmoothTime);

            spreadLaserObject.transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
        }

        public void HitLaser(GameObject hitObject)
        {
            if (hitObject != null && hitObject == playerObject)
            {
                laserCooldown.Refresh();
                laserSpawnerControllerB.Reset();
                laserSpawnerControllerB_L.Reset();
                laserSpawnerControllerB_R.Reset();
                laserSpawnerControllerB_LD.Reset();
                laserSpawnerControllerB_RD.Reset();
                onHitWithLaser?.Invoke(hitObject);
            }
        }
    }
}
