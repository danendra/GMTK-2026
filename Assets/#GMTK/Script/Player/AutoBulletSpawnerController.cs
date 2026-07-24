using UnityEngine;

using Anoa.Module;

namespace GMTK.Player
{
    public class AutoBulletSpawnerController : SpawnerController
    {
        [SerializeField] protected float fltDelay = 0.1f;
        [SerializeField] protected Vector2 direction = Vector2.up;

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
                Spawn();
                cooldown.Use();
            }
        }

        public override GameObject Spawn()
        {
            FixedDirectionController _objBullet = base.Spawn().GetComponent<FixedDirectionController>();

            _objBullet.SetDirection(direction);
            _objBullet.transform.parent = null;

            return _objBullet.gameObject;
        }
    }
}