using UnityEngine;

namespace GMTK
{
    public class RotateToDirectionController : MonoBehaviour
    {
        [SerializeField] protected InputController input;
        
        protected Vector2 direction;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(direction != input.directionInput)
            {
                direction = input.directionInput;

                transform.up = direction.normalized;
            }
        }
    }
}