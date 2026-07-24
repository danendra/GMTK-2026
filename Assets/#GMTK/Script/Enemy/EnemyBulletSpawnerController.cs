using UnityEngine;

using GMTK.Player;

namespace GMTK.Enemy
{
    public class EnemyBulletSpawnerController : SpawnerController
    {
        [SerializeField] protected Vector2 direction = Vector2.up;

        public override GameObject Spawn()
        {
            FixedDirectionController _objBullet = base.Spawn().GetComponent<FixedDirectionController>();

            _objBullet.SetDirection(direction);
            _objBullet.transform.parent = null;

            return _objBullet.gameObject;
        }
    }
}
