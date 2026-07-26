using System.Collections;
using UnityEngine;

namespace GMTK.Player
{
    public class DelayBeforeInactiveController : MonoBehaviour
    {
        [SerializeField] protected float fltTime = 2.0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        void OnEnable()
        {
            StartCoroutine(DelayInactive());
        }

        protected IEnumerator DelayInactive()
        {
            yield return new WaitForSeconds(fltTime);

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}