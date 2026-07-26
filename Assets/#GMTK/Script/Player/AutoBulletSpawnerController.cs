using UnityEngine;
using System.Collections.Generic;

using Anoa.Module;
using AudioSystem;

namespace GMTK.Player
{
    public class AutoBulletSpawnerController : SpawnerController
    {
        [SerializeField] protected float fltDelay = 0.1f;
        [SerializeField] protected Vector2 direction = Vector2.up;
        [SerializeField] protected SoundData soundData;
        [SerializeField] protected float shootSoundCooldown = 0.04f;

        protected CooldownModule cooldown;
        static readonly Dictionary<int, float> LastShootSoundTimeByRoot = new();

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
                int rootId = transform.root.GetInstanceID();
                float now = Time.time;
                float lastPlayTime = -999f;

                LastShootSoundTimeByRoot.TryGetValue(rootId, out lastPlayTime);
                if (now - lastPlayTime >= Mathf.Max(0f, shootSoundCooldown))
                {
                    LastShootSoundTimeByRoot[rootId] = now;

                    SoundManager.Instance.CreateSound()
                        .WithSoundData(soundData)
                        .WithRandomPitch()
                        .WithPosition(_objBullet.transform.position)
                        .Play();
                }
            }

            return _objBullet.gameObject;
        }
    }
}