using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK.UI
{
    using Enemy;

    public class UIBossHealthController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject goBossBarPanel;
        [SerializeField] protected Image imgHealthFill;
        [SerializeField] protected Image[] arrSkullImages;

        [Header("Skull Visuals")]
        [SerializeField] protected Color colorAlive = Color.white;
        [SerializeField] protected Color colorDead = new Color(1f, 1f, 1f, 0.2f);

        private BossHealthController bossHealthController;

        protected virtual void Update()
        {
            if (goBossBarPanel == null || !goBossBarPanel.activeSelf) return;
            if (bossHealthController == null) return;

            RefreshFill();
            RefreshSkulls();
        }

        public void ShowBar()
        {
            gameObject.SetActive(true);
            StartCoroutine(FindAndShowBar());
        }

        private IEnumerator FindAndShowBar()
        {
            Debug.Log("[UIBossHealthController] ShowBar called. Searching for BossHealthController...");
            float searchTime = 0f;
            
            bossHealthController = FindObjectOfType<BossHealthController>();
            while (bossHealthController == null)
            {
                searchTime += Time.unscaledDeltaTime;
                if (searchTime > 3f) // Check every 3 seconds
                {
                    Debug.LogWarning("[UIBossHealthController] Still searching for BossHealthController...");
                    
                    // Check if an EnemyHealthController exists instead (the old component)
                    EnemyHealthController anyEnemyHealth = FindObjectOfType<EnemyHealthController>();
                    if (anyEnemyHealth != null)
                    {
                        Debug.LogError($"[UIBossHealthController] ERROR: Found '{anyEnemyHealth.GetType().Name}' on '{anyEnemyHealth.gameObject.name}', but we need 'BossHealthController'! Please swap the component on this prefab/object.");
                    }
                    else
                    {
                        Debug.LogWarning("[UIBossHealthController] No health controllers of any type found in the scene yet. Waiting for boss to spawn...");
                    }
                    searchTime = 0f;
                }
                yield return null;
                bossHealthController = FindObjectOfType<BossHealthController>();
            }

            Debug.Log($"[UIBossHealthController] Boss found on '{bossHealthController.gameObject.name}'! Activating health bar.");
            if (goBossBarPanel != null)
                goBossBarPanel.SetActive(true);

            RefreshFill();
            RefreshSkulls();
        }

        public void HideBar()
        {
            if (goBossBarPanel != null)
                goBossBarPanel.SetActive(false);

            bossHealthController = null;
        }

        private void RefreshFill()
        {
            if (bossHealthController == null || imgHealthFill == null) return;

            float ratio = bossHealthController.MaxHealth > 0
                ? bossHealthController.CurrentHealth / bossHealthController.MaxHealth
                : 0f;

            imgHealthFill.fillAmount = Mathf.Clamp01(ratio);
        }

        private void RefreshSkulls()
        {
            if (arrSkullImages == null) return;

            int currentLives = bossHealthController != null
                ? bossHealthController.IntCurrentLives
                : 0;

            for (int i = 0; i < arrSkullImages.Length; i++)
            {
                if (arrSkullImages[i] == null) continue;
                arrSkullImages[i].color = i < currentLives ? colorAlive : colorDead;
            }
        }
    }
}
