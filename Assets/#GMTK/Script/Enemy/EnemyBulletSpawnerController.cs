using UnityEngine;

using AudioSystem;
using GMTK.Player;

namespace GMTK.Enemy
{
    public class EnemyBulletSpawnerController : SpawnerController
    {
        [SerializeField] protected Vector2 direction = Vector2.up;
        [SerializeField] protected SoundData soundData;

        public override GameObject Spawn()
        {
            FixedDirectionController _objBullet = base.Spawn().GetComponent<FixedDirectionController>();

            _objBullet.SetDirection(transform.TransformDirection(direction));
            _objBullet.transform.parent = null;

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
