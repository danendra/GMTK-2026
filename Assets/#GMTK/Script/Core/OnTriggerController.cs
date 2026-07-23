using UnityEngine;
using UnityEngine.Events;

namespace GMTK
{
    public class OnTriggerController : MonoBehaviour
    {
        [SerializeField] protected UnityEvent<GameObject> onTriggerEnter;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter2D(Collider2D _collision)
        {
            onTriggerEnter.Invoke(_collision.gameObject);
        }
    }
}