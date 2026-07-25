using Anoa.Module;
using UnityEngine;

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
    public class EnemyBulletType : MonoBehaviour
    {
        [SerializeField] protected EnemyBulletTypeEnum enemyBulletTypeEnum;
        [SerializeField] protected float fltDelay = 0.1f;
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

        protected CooldownModule cooldown;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cooldown = new CooldownModule(fltDelay, true);
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown.IsReady)
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
                cooldown.Use();
            }
        }
    }
}
