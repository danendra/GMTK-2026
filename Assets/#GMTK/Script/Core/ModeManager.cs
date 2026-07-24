using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Mode
{
    public class ModeManager : MonoBehaviour
    {
        [SerializeField] protected float fltMaxCountdown = 20;
        [SerializeField] protected UnityEvent onActiveModeStart;
        [SerializeField] protected UnityEvent onPassiveModeStart;

        public static ModeManager instance {get; protected set;}
        public bool isActive { get; protected set; }
        public float fltCountdown { get; protected set; }

        public UnityEvent GetActiveMode => onActiveModeStart;
        public UnityEvent GetPassiveMode => onPassiveModeStart;

        void Awake()
        {
            instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {            
            ChangeMode();
        }

        public void ChangeMode()
        {
            fltCountdown = fltMaxCountdown;
            isActive = !isActive;

            if (isActive)
                onActiveModeStart.Invoke();
            else
                onPassiveModeStart.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            fltCountdown -= Time.deltaTime;

            if (fltCountdown < 0)
            {
                ChangeMode();
            }
        }
    }
}