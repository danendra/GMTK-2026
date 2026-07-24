using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Player
{
    using Mode;

    public class PlayerModeController : MonoBehaviour
    {
        [SerializeField] protected UnityEvent onActiveMode;
        [SerializeField] protected UnityEvent onPassiveMode;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        void OnEnable()
        {
            ModeManager.instance.GetActiveMode.AddListener(ActiveMode);
            ModeManager.instance.GetPassiveMode.AddListener(PassiveMode);
        }

        void OnDisable()
        {
            ModeManager.instance.GetActiveMode.RemoveListener(ActiveMode);
            ModeManager.instance.GetPassiveMode.RemoveListener(PassiveMode);
        }

        public void ActiveMode()
        {
            onActiveMode.Invoke();
        }

        public void PassiveMode()
        {
            onPassiveMode.Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}