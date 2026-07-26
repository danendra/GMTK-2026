using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace GMTK.MainMenu
{
    /// <summary>
    /// A panel of the menu that can be navigated. The panel itself is toggled by the existing
    /// button wiring (GameObject.SetActive); this controller only watches which one is showing.
    /// </summary>
    [Serializable]
    public class MenuNavigationPanel
    {
        [Tooltip("Root GameObject of the panel, the same object the buttons toggle with SetActive.")]
        public GameObject panel;

        [Tooltip("Selectable focused when this panel opens. Leave empty to use the first interactable Selectable found under the panel.")]
        public GameObject firstSelected;

        [Tooltip("Button pressed when Cancel (Escape / gamepad B) is used while this panel is open. Usually the Back button.")]
        public Button cancelButton;

        [Tooltip("Extra reaction to Cancel while this panel is open. Runs in addition to the Cancel Button above.")]
        public UnityEvent onCancel;
    }

    /// <summary>
    /// Drives keyboard and gamepad navigation for a menu scene.
    /// <para>
    /// The Input System's <see cref="InputSystemUIInputModule"/> already turns the UI action map into
    /// move/submit/cancel events, but it only does so while the EventSystem has something selected.
    /// Menus that swap panels with SetActive lose that selection, so this controller keeps a valid
    /// selection at all times: it selects the first Selectable when a panel opens, restores focus when
    /// it is lost, and routes Cancel to the panel's Back button.
    /// </para>
    /// <para>
    /// Inherits <see cref="InputController"/>, so the inherited <c>arrBindings</c> array stays available
    /// for any extra per-scene input hooks.
    /// </para>
    /// </summary>
    public class MenuNavigationController : InputController
    {
        #region Serialized Fields

        [Header("Panels")]
        [Tooltip("Every panel of this menu. When several are active at once, the one drawn on top (highest sibling index) wins.")]
        [SerializeField] private MenuNavigationPanel[] _panels;

        [Header("Selection")]
        [Tooltip("Re-select the last valid Selectable when the selection is lost or its object is deactivated.")]
        [SerializeField] private bool _keepSelectionAlive = true;

        [Tooltip("Clear the selection highlight when the mouse is moved, so a hovered and a selected button are never highlighted at the same time.")]
        [SerializeField] private bool _clearSelectionOnPointer = true;

        [Tooltip("Stop the input module from clearing the selection when the player clicks the background.")]
        [SerializeField] private bool _keepSelectionOnBackgroundClick = true;

        [Tooltip("Pixels the mouse must travel before it counts as pointer input. Avoids tiny jitter stealing focus from the gamepad.")]
        [SerializeField] private float _pointerMoveThreshold = 4f;

        [Header("Cursor")]
        [Tooltip("Hide the hardware cursor while navigating with keyboard or gamepad, and show it again on mouse input.")]
        [SerializeField] private bool _hideCursorOnController = false;

        [Header("Debug")]
        [Tooltip("Prints the selection and device-mode changes to the console.")]
        [SerializeField] private bool _enableLogging = false;

        #endregion

        #region Properties

        /// <summary>True while the player is driving the menu with a keyboard or gamepad rather than the mouse.</summary>
        public bool IsControllerMode { get; private set; } = true;

        /// <summary>The panel currently showing, or null when every panel is hidden.</summary>
        public MenuNavigationPanel ActivePanel { get; private set; }

        #endregion

        #region Fields

        private GameObject _lastSelected;
        private Vector2 _lastPointerPosition;
        private bool _hasPointerPosition;
        private bool _subscribed;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // The UI map of this instance is separate from the one the input module drives, so
            // listening here never steals input from the module.
            inputAction.UI.Navigate.performed += HandleControllerInput;
            inputAction.UI.Submit.performed += HandleControllerInput;
            inputAction.UI.Cancel.performed += HandleCancelInput;
            inputAction.UI.Point.performed += HandlePointInput;
            inputAction.UI.Click.performed += HandleClickInput;
            _subscribed = true;
        }

        protected override void OnDestroy()
        {
            if (_subscribed)
            {
                inputAction.UI.Navigate.performed -= HandleControllerInput;
                inputAction.UI.Submit.performed -= HandleControllerInput;
                inputAction.UI.Cancel.performed -= HandleCancelInput;
                inputAction.UI.Point.performed -= HandlePointInput;
                inputAction.UI.Click.performed -= HandleClickInput;
                _subscribed = false;
            }

            base.OnDestroy();
        }

        // Start rather than Awake: the panels must have run their own Awake/Start (and
        // LevelSelectController must have set button interactability) before anything is selected.
        private void Start()
        {
            ApplyBackgroundClickBehaviour();

            ActivePanel = ResolveActivePanel();
            SelectFirstOf(ActivePanel);
        }

        private void Update()
        {
            MenuNavigationPanel current = ResolveActivePanel();

            if (current != ActivePanel)
            {
                ActivePanel = current;
                SelectFirstOf(current);
                return;
            }

            if (_keepSelectionAlive && IsControllerMode)
            {
                EnsureSelection();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Triggers the active panel's Cancel reaction. Also usable from a UnityEvent, for example
        /// from the inherited <c>arrBindings</c> array or from an on-screen Back button.
        /// </summary>
        public void Back()
        {
            MenuNavigationPanel panel = ActivePanel;
            if (panel == null) return;

            panel.onCancel?.Invoke();

            Button cancelButton = panel.cancelButton;
            if (cancelButton == null || !cancelButton.IsActive() || !cancelButton.IsInteractable()) return;

            // Submit rather than onClick.Invoke() so the button plays its normal press transition.
            ExecuteEvents.Execute(cancelButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            Log($"Cancel pressed '{cancelButton.name}'.");
        }

        /// <summary>Focuses a specific Selectable. Usable from a UnityEvent on a button.</summary>
        public void Select(GameObject target)
        {
            SetSelected(target);
        }

        /// <summary>Re-focuses the first Selectable of the panel currently showing.</summary>
        public void SelectFirstOfActivePanel()
        {
            SelectFirstOf(ResolveActivePanel());
        }

        #endregion

        #region Input Handling

        private void HandleControllerInput(InputAction.CallbackContext context)
        {
            EnterControllerMode();
        }

        private void HandleCancelInput(InputAction.CallbackContext context)
        {
            EnterControllerMode();
            Back();
        }

        private void HandleClickInput(InputAction.CallbackContext context)
        {
            // Click is also bound to touch and pen, so only a real pointer switches the mode.
            if (context.control?.device is Pointer)
            {
                EnterPointerMode();
            }
        }

        private void HandlePointInput(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();

            // The first sample only seeds the reference position; it is a resting cursor, not a move.
            if (!_hasPointerPosition)
            {
                _lastPointerPosition = position;
                _hasPointerPosition = true;
                return;
            }

            if ((position - _lastPointerPosition).sqrMagnitude < _pointerMoveThreshold * _pointerMoveThreshold) return;

            _lastPointerPosition = position;
            EnterPointerMode();
        }

        private void EnterControllerMode()
        {
            if (!IsControllerMode)
            {
                IsControllerMode = true;
                if (_hideCursorOnController) Cursor.visible = false;
                Log("Switched to keyboard/gamepad.");
            }

            EnsureSelection();
        }

        private void EnterPointerMode()
        {
            if (IsControllerMode)
            {
                IsControllerMode = false;
                if (_hideCursorOnController) Cursor.visible = true;
                Log("Switched to mouse.");
            }

            if (!_clearSelectionOnPointer) return;

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            // Remember what was focused so the next gamepad input can pick up where it left off.
            if (eventSystem.currentSelectedGameObject != null)
            {
                _lastSelected = eventSystem.currentSelectedGameObject;
                eventSystem.SetSelectedGameObject(null);
            }
        }

        #endregion

        #region Selection

        /// <summary>
        /// Returns the panel that should own the selection: the active one drawn on top.
        /// </summary>
        private MenuNavigationPanel ResolveActivePanel()
        {
            if (_panels == null) return null;

            MenuNavigationPanel best = null;
            int bestOrder = int.MinValue;

            for (int i = 0; i < _panels.Length; i++)
            {
                MenuNavigationPanel candidate = _panels[i];
                if (candidate?.panel == null || !candidate.panel.activeInHierarchy) continue;

                int order = candidate.panel.transform.GetSiblingIndex();
                if (order < bestOrder) continue;

                best = candidate;
                bestOrder = order;
            }

            return best;
        }

        private void SelectFirstOf(MenuNavigationPanel panel)
        {
            if (panel == null)
            {
                SetSelected(null);
                return;
            }

            GameObject target = panel.firstSelected;
            if (target == null || !target.activeInHierarchy || !IsSelectableTarget(target))
            {
                target = FindFirstSelectable(panel.panel);
            }

            SetSelected(target);
        }

        /// <summary>
        /// Falls back to hierarchy order, which matches the visual order inside a layout group.
        /// </summary>
        private GameObject FindFirstSelectable(GameObject root)
        {
            if (root == null) return null;

            Selectable[] selectables = root.GetComponentsInChildren<Selectable>(false);
            for (int i = 0; i < selectables.Length; i++)
            {
                Selectable selectable = selectables[i];
                if (selectable.IsInteractable() && selectable.navigation.mode != Navigation.Mode.None)
                {
                    return selectable.gameObject;
                }
            }

            return null;
        }

        private bool IsSelectableTarget(GameObject target)
        {
            Selectable selectable = target.GetComponent<Selectable>();
            return selectable != null && selectable.IsInteractable();
        }

        private void EnsureSelection()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            GameObject current = eventSystem.currentSelectedGameObject;
            if (current != null && current.activeInHierarchy)
            {
                _lastSelected = current;
                return;
            }

            if (_lastSelected != null && _lastSelected.activeInHierarchy && IsSelectableTarget(_lastSelected))
            {
                eventSystem.SetSelectedGameObject(_lastSelected);
                return;
            }

            SelectFirstOf(ActivePanel);
        }

        private void SetSelected(GameObject target)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                Debug.LogWarning("[MenuNavigationController] No EventSystem in the scene; the menu cannot be navigated.", this);
                return;
            }

            // Clearing first drops any stale pointer state left on the previous selection.
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(target);
            _lastSelected = target;

            Log(target == null ? "Selection cleared." : $"Selected '{target.name}'.");
        }

        private void ApplyBackgroundClickBehaviour()
        {
            if (!_keepSelectionOnBackgroundClick) return;

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            if (eventSystem.currentInputModule is InputSystemUIInputModule module)
            {
                module.deselectOnBackgroundClick = false;
            }
        }

        private void Log(string message)
        {
            if (!_enableLogging) return;
            Debug.Log($"[MenuNavigationController] {message}", this);
        }

        #endregion
    }
}
