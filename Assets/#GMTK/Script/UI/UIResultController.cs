using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GMTK.UI
{
    public class UIResultController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject successPanel;
        [SerializeField] private GameObject restartPanel;

        [Header("Success")]
        [SerializeField] private Graphic successText;
        [SerializeField] private float blinkDuration = 5f;
        [SerializeField] private float blinkSpeed = 0.25f;
        [SerializeField] private string nextSceneName = "Stage 2";

        [Header("Restart")]
        [SerializeField] private Button retryButton;

        Coroutine activeRoutine;

        void Awake()
        {
            HideAll();
        }

        public void ShowSuccess()
        {
            gameObject.SetActive(true);
            HideAll();

            if (successPanel != null)
            {
                successPanel.SetActive(true);
            }

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(IEShowSuccess());
        }

        public void ShowRestart()
        {
            gameObject.SetActive(true);
            HideAll();

            if (restartPanel != null)
            {
                restartPanel.SetActive(true);
            }

            if (retryButton != null && UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(retryButton.gameObject);
            }
        }

        public void RestartCurrentScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        IEnumerator IEShowSuccess()
        {
            float elapsed = 0f;
            bool visible = true;

            while (elapsed < blinkDuration)
            {
                if (successText != null)
                {
                    successText.enabled = visible;
                }

                visible = !visible;
                yield return new WaitForSecondsRealtime(blinkSpeed);
                elapsed += blinkSpeed;
            }

            if (successText != null)
            {
                successText.enabled = true;
            }

            Time.timeScale = 1f;

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }

            activeRoutine = null;
        }

        void HideAll()
        {
            if (successPanel != null)
            {
                successPanel.SetActive(false);
            }

            if (restartPanel != null)
            {
                restartPanel.SetActive(false);
            }
        }
    }
}
