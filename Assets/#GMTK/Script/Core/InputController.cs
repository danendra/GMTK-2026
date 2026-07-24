using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GMTK
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] protected InputBinding[] arrBindings;

        public Vector2 directionInput {get; protected set;}

        protected InputSystem_Actions inputAction;

        protected virtual void Awake()
        {
            inputAction = new InputSystem_Actions();

            inputAction.Player.Move.performed += _input => directionInput = _input.ReadValue<Vector2>();
            inputAction.Player.Move.canceled += _input => directionInput = Vector2.zero;

            for (int i = 0; i < arrBindings.Length; i++)
            {
                arrBindings[i].SetBinding();
            }                    
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < arrBindings.Length; i++)
            {
                arrBindings[i].RemoveBinding();
            } 
        }

        protected virtual void OnEnable()
        {
            inputAction?.Enable();

            for (int i = 0; i < arrBindings.Length; i++)
            {
                arrBindings[i].EnableBinding();
            } 
        }

        protected virtual void OnDisable()
        {
            inputAction?.Disable();

            for (int i = 0; i < arrBindings.Length; i++)
            {
                arrBindings[i].DisableBinding();
            } 
        }
    }

    [System.Serializable]
    public class InputBinding
    {
        public InputActionReference input;
        public UnityEvent onPerformed;
        public UnityEvent onCancel;

        public void SetBinding()
        {
            if (input == null || input.action == null) return;

            input.action.performed += _input => OnPerform(_input);
            input.action.canceled += _input => OnCancel(_input);
        }

        public void RemoveBinding()
        {
            if (input == null || input.action == null) return;

            input.action.performed -= _input => OnPerform(_input);
            input.action.canceled -= _input => OnCancel(_input);
        }

        public void EnableBinding()
        {
            if (input != null && input.action != null)
                input.action.Enable();
        }

        public void DisableBinding()
        {
            if (input != null && input.action != null)
                input.action.Disable();
        }

        private void OnPerform(InputAction.CallbackContext _intput) => onPerformed?.Invoke();
        private void OnCancel(InputAction.CallbackContext _input) => onCancel?.Invoke();
    }
}