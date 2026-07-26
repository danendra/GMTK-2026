using UnityEngine;

namespace GMTK
{
    using GMTK.Mode;
    using Player;
    public class BulletController : MonoBehaviour
    {
        protected PlayerLiveController playerLive;

        void Awake()
        {
            playerLive = FindAnyObjectByType<PlayerLiveController>();
        }

        void OnEnable()
        {
            if(!playerLive)
                Awake();

            playerLive?.AddRespawnListener(Disable);
            ModeManager.instance?.GetActiveMode.AddListener(Disable);
        }

        void OnDisable()
        {
            if(!playerLive)
                Awake();

            playerLive?.RemoveRespawnListener(Disable);
            ModeManager.instance?.GetActiveMode.RemoveListener(Disable);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }        
    }
}