using UnityEngine;

namespace GMTK.Player
{
    public class PlayerDeflectController : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected float fltDuration = 0.2f;

        protected Collider2D collider;
        protected Color color;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            collider = GetComponent<Collider2D>();
            
            color = sprite.color;
        }

        public void Activate()
        {
            if(!gameObject.activeInHierarchy) return;

            collider.enabled = true;            
            color.a = 1.0f;

            sprite.color = color;

            Invoke("Deactivate", fltDuration);
        }

        public void Deactivate()
        {
            collider.enabled = false;            
            color.a = 0.1f;

            sprite.color = color;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter2D(Collider2D _collision)
        {
            
        }
    }
}