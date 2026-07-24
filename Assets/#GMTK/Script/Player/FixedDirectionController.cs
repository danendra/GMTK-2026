using UnityEngine;

namespace GMTK.Player
{
    public class FixedDirectionController : InputController
    {
        [SerializeField] protected Vector2 direction;

        protected override void Awake()
        {
            directionInput = direction;
        }

        public void SetDirection(Vector2 _direction)
        {
            direction = _direction;
            
            Awake();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
    }
}