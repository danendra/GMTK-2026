using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Enemy
{
    public class BossHealthController : EnemyHealthController
    {
        [SerializeField] private int intMaxLives = 3;
        [SerializeField] private UnityEvent onLifeLost;

        public int IntCurrentLives { get; private set; }
        public int IntMaxLives => intMaxLives;

        protected override void Awake()
        {
            base.Awake();
            IntCurrentLives = intMaxLives;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            IntCurrentLives = intMaxLives;
            Debug.Log($"[BossHealthController] Enabled on '{gameObject.name}'. Max Lives: {intMaxLives}, Current Lives reset to: {IntCurrentLives}");
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            Debug.Log($"[BossHealthController] Hit! Damage: {damage}, HP: {currentHealth}/{maxHealth}, Lives: {IntCurrentLives}");
        }

        protected override void Die()
        {
            IntCurrentLives--;

            if (IntCurrentLives > 0)
            {
                currentHealth = maxHealth;
                onLifeLost?.Invoke();
                onHealthChanged?.Invoke();
            }
            else
            {
                // Auto-trigger success panel on final death (avoids prefab reference issue)
                UI.UISuccessController successController = FindObjectOfType<UI.UISuccessController>();
                if (successController != null)
                {
                    successController.Show();
                }
                else
                {
                    Debug.LogWarning("[BossHealthController] No UISuccessController found in the scene to show success panel.");
                }

                base.Die();
            }
        }
    }
}
