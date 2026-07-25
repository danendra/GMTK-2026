using UnityEngine;

namespace GMTK
{
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

            playerLive.AddRespawnListener(Disable);
        }

        void OnDisable()
        {
            playerLive.RemoveRespawnListener(Disable);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }        
    }
}