using UnityEngine;
using UnityEngine.Events;

using Anoa.Module;

namespace GMTK.Player
{
    public class PlayerDeflectController : MonoBehaviour
    {
        [SerializeField] protected float fltDelay = 0.2f;
        [SerializeField] protected float fltDuration = 0.2f;
        [SerializeField] protected SpawnerController spawnBullet;

        [SerializeField] protected UnityEvent onActive;
        [SerializeField] protected UnityEvent onDeactive;

        protected CooldownModule cooldownDeflect;                

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {            
            cooldownDeflect = new CooldownModule(fltDelay, true);            
        }

        public void Activate()
        {
            if(!gameObject.activeInHierarchy || !cooldownDeflect.IsReady) return;

            cooldownDeflect.Use();

            onActive.Invoke();

            Invoke("Deactivate", fltDuration);
        }

        public void Deactivate()
        {                      
            onDeactive.Invoke();
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