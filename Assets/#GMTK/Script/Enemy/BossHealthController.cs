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
                UI.UIResultController successController = FindObjectOfType<UI.UIResultController>();
                if (successController != null)
                {
                    successController.ShowSuccess();
                }
                else
                {
                    Debug.LogWarning("[BossHealthController] No UIResultController found in the scene to show success panel.");
                }

                base.Die();
            }
        }
    }
}
