using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace GMTK.UI
{
    public class UIPauseController : MonoBehaviour
    {
        [SerializeField] protected GameObject goPausePanel;
        [SerializeField] protected string strMainMenuSceneName = "MainMenu";

        public bool isPaused { get; protected set; }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;

            if (goPausePanel != null)
            {
                goPausePanel.SetActive(true);
            }
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;

            if (goPausePanel != null)
            {
                goPausePanel.SetActive(false);
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void RetryGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(strMainMenuSceneName);
        }

        protected virtual void Update()
        {
            bool isEscPressed = false;

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isEscPressed = true;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                isEscPressed = true;
            }

            if (isEscPressed)
            {
                TogglePause();
            }
        }
    }
}


