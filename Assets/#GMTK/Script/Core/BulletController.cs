using UnityEngine;

namespace GMTK
{
    public class BulletController : MonoBehaviour
    {
        void OnEnable()
        {
            BulletManager.instance?.AddBullet(gameObject);
        }

        public void Disable()
        {
            BulletManager.instance?.RemoveBullet(gameObject);
        }
    }
}