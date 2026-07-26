using UnityEngine;

using Anoa.Module;
using AudioSystem;

namespace GMTK.Player
{
    public class AutoBulletSpawnerController : SpawnerController
    {
        [SerializeField] protected float fltDelay = 0.1f;
        [SerializeField] protected Vector2 direction = Vector2.up;
        [SerializeField] protected SoundData soundData;

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

            if (soundData != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.CreateSound()
                    .WithSoundData(soundData)
                    .WithRandomPitch()
                    .WithPosition(_objBullet.transform.position)
                    .Play();
            }

            return _objBullet.gameObject;
        }
    }
}