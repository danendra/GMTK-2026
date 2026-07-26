using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GMTK.UI
{
    public class UIGameOverController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject goGameOverPanel;
        [SerializeField] private Button btnRetry;
        [SerializeField] private Button btnMainMenu;

        [Header("Settings")]
        [SerializeField] private string strMainMenuSceneName = "MainMenu";

        private void OnEnable()
        {
            if (btnRetry != null) btnRetry.onClick.AddListener(HandleRetry);
            if (btnMainMenu != null) btnMainMenu.onClick.AddListener(HandleMainMenu);
        }

        private void OnDisable()
        {
            if (btnRetry != null) btnRetry.onClick.RemoveListener(HandleRetry);
            if (btnMainMenu != null) btnMainMenu.onClick.RemoveListener(HandleMainMenu);
        }

        public void Show()
        {
            if (goGameOverPanel != null)
                goGameOverPanel.SetActive(true);

            // Auto-select retry button for keyboard navigation
            if (btnRetry != null && UnityEngine.EventSystems.EventSystem.current != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(btnRetry.gameObject);
            }
        }

        public void Hide()
        {
            if (goGameOverPanel != null)
                goGameOverPanel.SetActive(false);
        }

        private void HandleRetry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(strMainMenuSceneName);
        }
    }
}
