using UnityEngine;
using MoreMountains.Feedbacks;

namespace GMTK.Player
{
    using Enemy;

    public class PlayerBombController : MonoBehaviour
    {
        protected PlayerLiveController playerLive;
        protected InputSystem_Actions inputAction;
        
        [Header("References")]
        [SerializeField] protected MMFeedbacks bombFeedbacks;

        [Header("Bomb Settings")]
        [SerializeField] protected float fltMaxCharge = 100f;
        [SerializeField] protected float fltPassiveChargeRate = 2f;
        [SerializeField] protected float fltBombDamage = 50f;

        private float fltCurrentCharge = 0f;

        public float FltCurrentCharge => fltCurrentCharge;
        public float FltMaxCharge => fltMaxCharge;
        public bool IsReady => fltCurrentCharge >= fltMaxCharge;

        protected virtual void Awake()
        {
            playerLive = GetComponentInParent<PlayerLiveController>();
            inputAction = new InputSystem_Actions();

            inputAction.Player.Power.performed += _ => TriggerBomb();
        }

        protected virtual void OnEnable()
        {
            inputAction?.Enable();
        }

        protected virtual void OnDisable()
        {
            inputAction?.Disable();
        }

        protected virtual void Update()
        {
            if (fltCurrentCharge < fltMaxCharge)
            {
                AddCharge(fltPassiveChargeRate * Time.deltaTime);
            }
        }

        public void AddCharge(float amount)
        {
            fltCurrentCharge = Mathf.Clamp(fltCurrentCharge + amount, 0f, fltMaxCharge);
        }

        public virtual void TriggerBomb()
        {
            if (!IsReady)
            {
                Debug.Log($"[PlayerBombController] Bomb not ready! Charge: {fltCurrentCharge:F0}/{fltMaxCharge:F0}");
                return;
            }

            Debug.Log("Tombol X (Bomb) ditekan!");
            bombFeedbacks?.PlayFeedbacks();
            
            BulletController[] arrBullets = FindObjectsOfType<BulletController>();
            int intBulletsCleared = 0;
            for (int i = 0; i < arrBullets.Length; i++)
            {
                if (arrBullets[i].CompareTag("Enemy"))
                {
                    arrBullets[i].Disable();
                    intBulletsCleared++;
                }
            }

            EnemyHealthController[] arrEnemies = FindObjectsOfType<EnemyHealthController>();
            for (int i = 0; i < arrEnemies.Length; i++)
            {
                arrEnemies[i].TakeDamage(fltBombDamage);
            }

            Debug.Log($"[PlayerBombController] Bomb cleared {intBulletsCleared} bullets and damaged {arrEnemies.Length} enemies.");

            fltCurrentCharge = 0f;
            
            if (playerLive == null)
            {
                playerLive = GetComponentInParent<PlayerLiveController>();
            }
            if (playerLive == null)
            {
                playerLive = FindAnyObjectByType<PlayerLiveController>();
            }
            
            if (playerLive)
            {
                playerLive.TriggerBomb();
            }
        }
    }
}
