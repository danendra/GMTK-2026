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

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player enemyDamagedEffect;
        [SerializeField] private MMF_Player enemyDeadEffect;

        protected float bulletDamage = 1f;

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
                    SoundManager.Instance.CreateSound()
                        .WithSoundData(hitSoundData)
                        .WithRandomPitch()
                        .WithPosition(_collision.transform.position)
                        .Play();
                }

                enemyDamagedEffect.PlayFeedbacks();
                TakeDamage(bulletDamage);
            }
        }

        protected override void Die()
        {
            base.Die();

            enemyDeadEffect.PlayFeedbacks();
            if (animator)
            {
                animator.SetBool(isDestroyBoolName, true);
            }
            else
            {
                gameObject.SetActive(false);
            }
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
