using UnityEngine;
using AudioSystem;
using MoreMountains.Feedbacks;

namespace GMTK.Enemy
{
    public class EnemyHealthController : HealthController
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected string isDestroyBoolName = "IsDestroy";
        [SerializeField] protected string playerTag = "Player";
        [SerializeField] protected SoundData hitSoundData;
        [SerializeField] protected float hitSoundCooldown = 0.04f;
        [SerializeField] private MMF_Player damagedEffect;

        protected float bulletDamage = 1f;
        protected float lastHitSoundTime = -999f;

        protected override void Awake()
        {
            base.Awake();
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag(playerTag))
            {
                if (hitSoundData != null && SoundManager.Instance != null)
                {
                    float now = Time.time;
                    if (now - lastHitSoundTime >= Mathf.Max(0f, hitSoundCooldown))
                    {
                        lastHitSoundTime = now;

                        SoundManager.Instance.CreateSound()
                            .WithSoundData(hitSoundData)
                            .WithRandomPitch()
                            .WithPosition(_collision.transform.position)
                            .Play();
                    }
                }
                if (damagedEffect != null)
                {
                    damagedEffect.PlayFeedbacks();
                }
                TakeDamage(bulletDamage);
            }
        }

        protected override void Die()
        {
            base.Die();

            // if (animator)
            // {
            //     animator.SetBool(isDestroyBoolName, true);
            // }
            // else
            // {
            //     gameObject.SetActive(false);
            // }
        }

        /// <summary>
        /// Call this method from Animation Event at the end of the destroy animation
        /// or via UnityEvent onDeath.
        /// </summary>
        public virtual void OnDestroyAnimationFinished()
        {
            gameObject.SetActive(false);
        }
    }
}
