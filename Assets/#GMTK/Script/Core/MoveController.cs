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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            if (direction != input.directionInput)
            {
                direction = input.directionInput;

                rb.linearVelocity = direction * fltSpeed;
            }
        }
    }
}