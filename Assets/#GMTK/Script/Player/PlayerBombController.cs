using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

namespace GMTK.Player
{
    public class PlayerBombController : MonoBehaviour
    {
        protected PlayerLiveController playerLive;        
        
        [Header("References")]
        [SerializeField] protected MMFeedbacks bombFeedbacks;

        [Header("Bomb Settings")]
        [SerializeField] protected float fltMaxCharge = 100f;
        [SerializeField] protected float fltPassiveChargeRate = 2f;    
        [SerializeField] UnityEvent onBombTrigger;

        private float fltCurrentCharge = 0f;

        public float FltCurrentCharge => fltCurrentCharge;
        public float FltMaxCharge => fltMaxCharge;
        public bool IsReady => fltCurrentCharge >= fltMaxCharge;
        public UnityEvent OnBombTrigger => onBombTrigger;

        protected virtual void Awake()
        {
            playerLive = GetComponentInParent<PlayerLiveController>();            
        }

        void Start()
        {
            fltCurrentCharge = fltMaxCharge;
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
                return;
            }
            
            bombFeedbacks?.PlayFeedbacks();
            onBombTrigger.Invoke();
                        
            fltCurrentCharge = 0f;                        
        }
    }
}
