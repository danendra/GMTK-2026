using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GMTK.UI
{
    public class UITutorialController : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The parent GameObject of the tutorial panel/canvas.")]
        [SerializeField] protected GameObject tutorialPanel;

        [Tooltip("The UI Image component used to display the active tutorial sprite slide.")]
        [SerializeField] protected Image tutorialImageDisplay;

        [Tooltip("The Button component used to close/advance the tutorial slides. The script will dynamically bind click events to this button.")]
        [SerializeField] protected Button advanceButton;

        [Header("Tutorial Slides")]
        [Tooltip("Array of Sprites representing the tutorial slides/steps in order.")]
        [SerializeField] protected Sprite[] tutorialSprites;

        [Header("Configuration")]
        [Tooltip("If true, automatically starts/shows the tutorial when the scene starts.")]
        [SerializeField] protected bool playOnStart = true;

        [Tooltip("If true, automatically freezes the game (Time.timeScale = 0) when the tutorial is active.")]
        [SerializeField] protected bool freezeTime = true;

        [Tooltip("If true, resumes the game time (Time.timeScale = 1) when the tutorial closes. Turn this OFF (false) if you want to keep the game frozen for a subsequent dialogue panel.")]
        [SerializeField] protected bool resumeTimeOnEnd = false;

        [Header("PlayerPrefs Configuration")]
        [Tooltip("If true, tutorial will save completed status in PlayerPrefs and skip showing on subsequent game starts.")]
        [SerializeField] protected bool saveToPlayerPrefs = true;

        [Tooltip("PlayerPrefs key to store if tutorial has been completed.")]
        [SerializeField] protected string playerPrefsKey = "GMTK_Tutorial_Played";

        [Header("Events")]
        [Tooltip("Fired when the tutorial ends/completes. Hook this up to play the Dialogue Panel next!")]
        [SerializeField] protected UnityEvent onTutorialEnd;

        protected int currentSlideIndex = 0;
        protected bool hasPlayed = false;

        protected virtual void Start()
        {
            if (playOnStart)
            {
                StartTutorial();
            }
            else
            {
                // Ensure panel starts hidden if not playing automatically
                if (tutorialPanel != null)
                {
                    tutorialPanel.SetActive(false);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            // Clean up listener just in case
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceTutorial);
            }
        }

        /// <summary>
        /// Starts and displays the tutorial panel from the first slide.
        /// </summary>
        public void StartTutorial()
        {
            // If already played during this gameplay session, do nothing and return safely
            if (hasPlayed)
            {
                if (tutorialPanel != null)
                {
                    tutorialPanel.SetActive(false);
                }
                return;
            }

            // Check if player already completed the tutorial and save to PlayerPrefs is active
            if (saveToPlayerPrefs && PlayerPrefs.GetInt(playerPrefsKey, 0) == 1)
            {
                hasPlayed = true; // Mark as played to skip future manual triggers
                if (tutorialPanel != null)
                {
                    tutorialPanel.SetActive(false);
                }
                onTutorialEnd.Invoke();
                return;
            }

            hasPlayed = true;

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }

            if (freezeTime)
            {
                Time.timeScale = 0f;
            }

            // Bind click listener dynamically to this controller instance
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceTutorial);
                advanceButton.onClick.AddListener(AdvanceTutorial);
            }

            currentSlideIndex = 0;
            ShowSlide(currentSlideIndex);
        }

        /// <summary>
        /// Advances to the next tutorial slide or completes the tutorial if there are no more slides.
        /// </summary>
        public void AdvanceTutorial()
        {
            currentSlideIndex++;
            ShowSlide(currentSlideIndex);
        }

        /// <summary>
        /// Updates the displayed tutorial sprite according to index.
        /// </summary>
        protected void ShowSlide(int index)
        {
            if (tutorialSprites == null || tutorialSprites.Length == 0)
            {
                CompleteTutorial();
                return;
            }

            if (index < 0 || index >= tutorialSprites.Length)
            {
                CompleteTutorial();
                return;
            }

            if (tutorialImageDisplay != null)
            {
                tutorialImageDisplay.sprite = tutorialSprites[index];
                tutorialImageDisplay.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Closes the tutorial, resumes time scale if configured, saves progress in PlayerPrefs, and triggers final events.
        /// </summary>
        public void CompleteTutorial()
        {
            // Unbind listener when tutorial closes
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceTutorial);
            }

            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }

            if (freezeTime && resumeTimeOnEnd)
            {
                Time.timeScale = 1f;
            }

            if (saveToPlayerPrefs)
            {
                PlayerPrefs.SetInt(playerPrefsKey, 1);
                PlayerPrefs.Save();
            }

            onTutorialEnd.Invoke();
        }

        /// <summary>
        /// Resets the PlayerPrefs completion state for testing/debugging.
        /// </summary>
        [ContextMenu("Reset PlayerPrefs Key")]
        public void ResetPlayerPrefsKey()
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
            PlayerPrefs.Save();
            hasPlayed = false; // Reset session status as well
            Debug.Log($"PlayerPrefs key '{playerPrefsKey}' and runtime session status have been reset.");
        }
    }
}
