using UnityEngine;

namespace GMTK.UI
{
    using Player;

    public class UIPlayerLivesController : MonoBehaviour
    {
        [SerializeField] protected PlayerLiveController playerLiveController;
        [SerializeField] protected Transform heartsRoot;
        [SerializeField] protected GameObject[] arrHeartObjects = new GameObject[5];
        [SerializeField] protected int totalHearts = 5;

        protected int lastLives = -1;

        protected virtual void Awake()
        {
            CacheHeartsFromRoot();
        }

        protected virtual void Start()
        {
            Refresh();
        }

        protected virtual void Update()
        {
            if (playerLiveController == null)
            {
                return;
            }

            if (playerLiveController.intCurrentHealth != lastLives)
            {
                Refresh();
            }
        }
        
        public void Refresh()
        {
            if (playerLiveController == null || arrHeartObjects == null)
            {
                return;
            }

            int currentLives = Mathf.Clamp(playerLiveController.intCurrentHealth, 0, totalHearts);
            lastLives = playerLiveController.intCurrentHealth;

            int loopCount = Mathf.Min(totalHearts, arrHeartObjects.Length);
            for (int i = 0; i < loopCount; i++)
            {
                if (arrHeartObjects[i] == null)
                {
                    continue;
                }

                arrHeartObjects[i].SetActive(i < currentLives);
            }
        }

        protected void CacheHeartsFromRoot()
        {
            if (heartsRoot == null)
            {
                return;
            }

            int loopCount = Mathf.Min(totalHearts, arrHeartObjects.Length, heartsRoot.childCount);
            for (int i = 0; i < loopCount; i++)
            {
                arrHeartObjects[i] = heartsRoot.GetChild(i).gameObject;
            }
        }
    }
}
