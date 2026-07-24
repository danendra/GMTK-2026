using UnityEngine;

using TMPro;

namespace GMTK.UI
{
    using Mode;

    public class UITimerController : MonoBehaviour
    {
        [SerializeField] protected TMP_Text txtTimer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            txtTimer.text = ModeManager.instance.fltCountdown.ToString("F0");
        }
    }
}