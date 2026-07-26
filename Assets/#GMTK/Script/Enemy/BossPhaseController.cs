using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Enemy
{
    public class BossPhaseController : MonoBehaviour
    {
        [SerializeField] protected UnityEvent[] arrOnChangePhase;

        [SerializeField] protected int intCurrentPhase = -1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ChangePhase();
        }

        public void ChangePhase()
        {
            intCurrentPhase++;

            arrOnChangePhase[intCurrentPhase].Invoke();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}