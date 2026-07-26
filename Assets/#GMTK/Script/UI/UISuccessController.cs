using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GMTK.UI
{
    /// <summary>
    /// Controls the Success Panel UI sequence.
    /// Shows the panel, blinks the success text for a duration, then loads the next scene.
    /// Wire the BossHealthController's onDeath UnityEvent to Show() in the Inspector.
    /// </summary>
    public class UISuccessController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject goSuccessPanel;
        [SerializeField] private Graphic txtSuccessMessage; // Graphic supports both TMPro and standard UI Text!

        [Header("Blink Settings")]
        [SerializeField] private float fltBlinkDuration = 5.0f;
        [SerializeField] private float fltBlinkSpeed = 0.25f;

        [Header("Next Level Setting")]
        [SerializeField] private string strNextLevelSceneName = "Stage 2";

        private void Awake()
        {
            if (goSuccessPanel != null)
                goSuccessPanel.SetActive(false);
        }

        /// <summary>
        /// Call this from BossHealthController's final onDeath event or stage success event.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true); // Wakes up the GameObject so it can run the coroutine

            Debug.Log("[UISuccessController] Show() called! Panel ref: " + goSuccessPanel);
            if (goSuccessPanel != null)
                goSuccessPanel.SetActive(true);
            else
                Debug.LogError("[UISuccessController] goSuccessPanel is NULL! Assign it in the Inspector!");

            StartCoroutine(IEPlaySuccessSequence());
        }

        private IEnumerator IEPlaySuccessSequence()
        {
            float elapsed = 0f;
            bool isVisible = true;

            // Loop for the duration, toggling the text visibility
            while (elapsed < fltBlinkDuration)
            {
                if (txtSuccessMessage != null)
                {
                    txtSuccessMessage.enabled = isVisible;
                }

                isVisible = !isVisible;
                yield return new WaitForSecondsRealtime(fltBlinkSpeed);
                elapsed += fltBlinkSpeed;
            }

            // Ensure the text is visible at the end of blinking
            if (txtSuccessMessage != null)
            {
                txtSuccessMessage.enabled = true;
            }

            // Unpause the game timescale just in case
            Time.timeScale = 1f;

            // Load the next level scene
            if (!string.IsNullOrEmpty(strNextLevelSceneName))
            {
                SceneManager.LoadScene(strNextLevelSceneName);
            }
            else
            {
                Debug.LogWarning("[UISuccessController] Next level scene name is empty. Cannot load next stage.");
            }
        }
    }
}
