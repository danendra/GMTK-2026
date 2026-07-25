using UnityEngine;
using UnityEngine.Events;

namespace GMTK
{
    public class HealthController : MonoBehaviour, IHealth, IDamageTake
    {
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected UnityEvent onHealthChanged;
        [SerializeField] protected UnityEvent onDeath;

        protected float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        protected virtual void OnEnable()
        {
            currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            onHealthChanged?.Invoke();

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        protected virtual void Die()
        {
            onDeath?.Invoke();
        }
    }
}
