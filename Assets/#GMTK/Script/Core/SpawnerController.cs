using UnityEngine;

using Anoa.Module;

namespace GMTK
{
    public class SpawnerController : MonoBehaviour
    {
        [SerializeField] protected Vector3 offset;
        [SerializeField] protected PoolerContainer pooler;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        public virtual GameObject Spawn()
        {
            GameObject _obj = pooler.Pop();
            _obj.transform.position = transform.position;

            return _obj;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.aliceBlue;
            Gizmos.DrawWireSphere(transform.position + offset, .1f);
        }
    }
}