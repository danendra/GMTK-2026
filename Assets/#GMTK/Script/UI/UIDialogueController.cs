using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace GMTK.UI
{
    [System.Serializable]
    public struct DialogueLine
    {
        [Tooltip("The name of the character speaking this line.")]
        public string speakerName;

        [Tooltip("The text content of this dialogue line.")]
        [TextArea(3, 5)]
        public string textContent;
    }

    public class UIDialogueController : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The parent GameObject of the dialogue panel/canvas.")]
        [SerializeField] protected GameObject dialoguePanel;
        
        [Tooltip("TextMeshPro text component for the dialogue content.")]
        [SerializeField] protected TextMeshProUGUI content;
        
        [Tooltip("TextMeshPro text component for the speaker's name (optional).")]
        [SerializeField] protected TextMeshProUGUI speaker;

        [Tooltip("The Button component used to click/advance the dialogue. The script will dynamically bind click events to this button.")]
        [SerializeField] protected Button advanceButton;

        [Header("Dialogue Configuration")]
        [Tooltip("List of dialogue lines to be displayed in sequence, combining speaker and text.")]
        [SerializeField] protected DialogueLine[] dialogueLines;

        [Tooltip("Speed of typewriter effect (delay in seconds between characters).")]
        [SerializeField] protected float textSpeed = 0.03f;

        [Tooltip("If true, automatically starts the dialogue when the scene starts. Turn OFF if you want to trigger this dialogue manually (e.g., when a boss spawns or after a tutorial).")]
        [SerializeField] protected bool playOnStart = true;

        [Tooltip("If true, automatically freezes the game (Time.timeScale = 0) when the dialogue plays and resumes it (Time.timeScale = 1) when it finishes.")]
        [SerializeField] protected bool freezeTime = true;

        [Header("PlayerPrefs Configuration")]
        [Tooltip("If true, dialogue will save completed status in PlayerPrefs and skip showing on subsequent game starts.")]
        [SerializeField] protected bool saveToPlayerPrefs = true;

        [Tooltip("PlayerPrefs key to store if dialogue has been completed.")]
        [SerializeField] protected string playerPrefsKey = "GMTK_Dialogue_Played";

        [Header("Events")]
        [Tooltip("Fired when the dialogue sequence ends/completes.")]
        [SerializeField] protected UnityEvent onDialogueEnd;

        protected int currentLineIndex = 0;
        protected bool isTyping = false;
        protected bool hasPlayed = false;
        protected Coroutine typewriterCoroutine;

        protected virtual void Start()
        {
            if (playOnStart)
            {
                // Start dialogue sequence automatically if configured
                StartDialogue();
            }
            else
            {
                // Ensure panel starts hidden if not playing automatically
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(false);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            // Clean up listener just in case
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceDialogue);
            }
        }

        /// <summary>
        /// Starts or resets the dialogue sequence from the beginning.
        /// </summary>
        public void StartDialogue()
        {
            // If already played during this gameplay session, do nothing and return safely
            if (hasPlayed)
            {
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(false);
                }
                return;
            }

            // Check if player already completed the dialogue and save to playerprefs is active
            if (saveToPlayerPrefs && PlayerPrefs.GetInt(playerPrefsKey, 0) == 1)
            {
                hasPlayed = true; // Mark as played to skip future manual triggers
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(false);
                }
                
                // If the game was frozen by the tutorial, we must ensure it resumes when dialogue is skipped
                if (freezeTime)
                {
                    Time.timeScale = 1f;
                }
                
                onDialogueEnd.Invoke();
                return;
            }

            hasPlayed = true;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            if (freezeTime)
            {
                Time.timeScale = 0f;
            }

            // Bind click listener dynamically to this controller instance
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceDialogue);
                advanceButton.onClick.AddListener(AdvanceDialogue);
            }

            currentLineIndex = 0;
            ShowLine(currentLineIndex);
        }

        /// <summary>
        /// Advances the dialogue text. If still typing, skips typing animation to show full line.
        /// If line is fully shown, advances to the next line or completes.
        /// </summary>
        public void AdvanceDialogue()
        {
            if (isTyping)
            {
                if (typewriterCoroutine != null)
                {
                    StopCoroutine(typewriterCoroutine);
                    typewriterCoroutine = null;
                }

                if (dialogueLines != null && currentLineIndex >= 0 && currentLineIndex < dialogueLines.Length)
                {
                    content.text = dialogueLines[currentLineIndex].textContent;
                }
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                ShowLine(currentLineIndex);
            }
        }

        /// <summary>
        /// Displays a line by starting the typewriter coroutine.
        /// </summary>
        protected void ShowLine(int index)
        {
            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                CompleteDialogue();
                return;
            }

            if (index < 0 || index >= dialogueLines.Length)
            {
                CompleteDialogue();
                return;
            }

            // Set speaker name if speaker element exists
            if (speaker != null)
            {
                string speakerName = dialogueLines[index].speakerName;
                if (!string.IsNullOrEmpty(speakerName))
                {
                    speaker.text = speakerName;
                    speaker.gameObject.SetActive(true);
                }
                else
                {
                    speaker.text = "";
                    speaker.gameObject.SetActive(false);
                }
            }

            // Start typewriter coroutine
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = StartCoroutine(TypewriteText(dialogueLines[index].textContent));
        }

        /// <summary>
        /// Coroutine that prints text character-by-character.
        /// Uses WaitForSecondsRealtime to function correctly even if Time.timeScale is 0.
        /// </summary>
        protected IEnumerator TypewriteText(string text)
        {
            isTyping = true;
            content.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                content.text += text[i];
                yield return new WaitForSecondsRealtime(textSpeed);
            }

            isTyping = false;
            typewriterCoroutine = null;
        }

        /// <summary>
        /// Finishes the dialogue sequence, saves the completion state to PlayerPrefs,
        /// deactivates the panel, and invokes the ending events.
        /// </summary>
        public void CompleteDialogue()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            // Unbind listener when dialogue panel closes
            if (advanceButton != null)
            {
                advanceButton.onClick.RemoveListener(AdvanceDialogue);
            }

            isTyping = false;

            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            if (freezeTime)
            {
                Time.timeScale = 1f;
            }

            if (saveToPlayerPrefs)
            {
                PlayerPrefs.SetInt(playerPrefsKey, 1);
                PlayerPrefs.Save();
            }

            onDialogueEnd.Invoke();
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
