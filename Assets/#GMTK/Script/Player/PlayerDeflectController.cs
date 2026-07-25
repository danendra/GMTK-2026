using UnityEngine;

using Anoa.Module;

namespace GMTK.Player
{
    public class PlayerDeflectController : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer sprite;
        [SerializeField] protected float fltDelay = 0.2f;
        [SerializeField] protected float fltDuration = 0.2f;
        [SerializeField] protected SpawnerController spawnBullet;

        protected CooldownModule cooldownDeflect;
        protected Collider2D collider;
        protected Color color;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            collider = GetComponent<Collider2D>();
            cooldownDeflect = new CooldownModule(fltDelay, true);
            
            color = sprite.color;
        }

        public void Activate()
        {
            if(!gameObject.activeInHierarchy || !cooldownDeflect.IsReady) return;

            collider.enabled = true;            
            color.a = 1.0f;

            sprite.color = color;

            cooldownDeflect.Use();

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
            if (_collision.tag == "Enemy")
            {
                FixedDirectionController  _direction = spawnBullet.Spawn().GetComponent<FixedDirectionController>();
                Vector2 _directionReflect = (_collision.transform.position - transform.parent.position).normalized;

                _direction.transform.position = _collision.transform.position;
                _direction.SetDirection(_directionReflect);
            }
        }
    }
}