using Unity.VisualScripting;
using UnityEngine;

namespace GMTK
{
    public class MoveController : MonoBehaviour
    {
        [SerializeField] protected InputController input;
        [SerializeField] protected float fltSpeed = 10;

        protected Rigidbody2D rb;
        protected Vector2 direction;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void ChangeSpeed(float _fltSpeed)
        {
            fltSpeed = _fltSpeed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!rb)
            {
                if (direction != input.directionInput)
                {
                    direction = input.directionInput;
                }

                transform.Translate(direction * fltSpeed * Time.deltaTime, Space.World);
            }
        }

        protected void FixedUpdate()
        {
            if (rb)
            {
                if (direction != input.directionInput)
                {
                    direction = input.directionInput;
                }

                rb.linearVelocity = direction * fltSpeed;
            }
        }
    }
}