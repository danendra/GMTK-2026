using UnityEngine;
using UnityEngine.UI;

namespace GMTK.UI
{
    using Player;

    public class UIPlayerLivesController : MonoBehaviour
    {
        [SerializeField] protected PlayerLiveController playerLiveController;
        [SerializeField] protected Image[] arrHeartImages;

        [Header("Heart Visuals")]
        [SerializeField] protected Color colorAlive = Color.white;
        [SerializeField] protected Color colorDead = new Color(1f, 1f, 1f, 0.2f);

        protected virtual void Start()
        {
            Refresh();
        }
        
        public void Refresh()
        {
            if (playerLiveController == null || arrHeartImages == null) return;

            int currentLives = playerLiveController.intCurrentHealth;

            for (int i = 0; i < arrHeartImages.Length; i++)
            {
                if (arrHeartImages[i] == null) continue;
                arrHeartImages[i].color = i < currentLives ? colorAlive : colorDead;
            }
        }
    }
}
