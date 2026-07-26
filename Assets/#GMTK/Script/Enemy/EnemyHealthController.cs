using UnityEngine;
using AudioSystem;

namespace GMTK.Enemy
{
    public class EnemyHealthController : HealthController
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected string isDestroyBoolName = "IsDestroy";
        [SerializeField] protected string playerTag = "Player";        
        [SerializeField] protected SoundData hitSoundData;

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
