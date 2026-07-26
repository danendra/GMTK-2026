using UnityEngine;
using MoreMountains.Feedbacks;

namespace GMTK.Player
{
    public class PlayerBombController : MonoBehaviour
    {
        protected PlayerLiveController playerLive;
        protected InputSystem_Actions inputAction;
        
        [SerializeField] protected MMFeedbacks bombFeedbacks;

        protected virtual void Awake()
        {
            playerLive = GetComponentInParent<PlayerLiveController>();
            inputAction = new InputSystem_Actions();

            // The 'Power' action is bound to the 'X' key in the Input System
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

        public virtual void TriggerBomb()
        {
            Debug.Log("Tombol X (Bomb) ditekan!");
            bombFeedbacks?.PlayFeedbacks();
            
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
            else
            {
                Debug.LogError("Gagal memanggil Bomb: PlayerLiveController tidak ditemukan sama sekali!");
            }
        }
    }
}
