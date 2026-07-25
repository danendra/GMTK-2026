using UnityEngine;

namespace GMTK.Enemy
{
    public class EnemyLevel2Basic : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Initial velocity (X = Horizontal, Y = Vertical). A negative Y value means moving downwards.")]
        [SerializeField] private Vector2 initialVelocity = new Vector2(3f, -5f);
        
        [Tooltip("How fast the plane 'pulls up' to fly straight.")]
        [SerializeField] private float pullUpAcceleration = 3f; 
        
        [Tooltip("The final target Y velocity (0 = flying straight horizontally).")]
        [SerializeField] private float targetYVelocity = 0f; 
        
        [Header("Rotation Settings")]
        [Tooltip("Aktifkan agar pesawat otomatis berputar menghadap arah geraknya.")]
        [SerializeField] private bool lookForward = true;
        
        [Tooltip("Rotation offset (change this if your plane's default sprite does not face right).")]
        [SerializeField] private float rotationOffset = 0f; 

        [Header("Boundary Settings")]
        [Tooltip("Batas posisi X di mana pesawat akan dimatikan (kembali ke pool).")]
        [SerializeField] private float disableXBoundary = 15f; 

        private Vector2 currentVelocity;
        private bool hasSetDirection;

        void OnEnable()
        {
            hasSetDirection = false;
        }

        public void SetTargetPoint(Vector2 targetPos)
        {
            // Menghitung arah dari posisi sekarang ke target
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            
            // Mengatur kecepatan X menuju target
            float currentSpeedX = Mathf.Abs(initialVelocity.x);
            initialVelocity.x = direction.x > 0 ? currentSpeedX : -currentSpeedX;
            
            // Kecepatan akhir Y (targetYVelocity) diarahkan lurus ke target
            float totalSpeed = initialVelocity.magnitude;
            targetYVelocity = direction.y * totalSpeed;

            currentVelocity = initialVelocity;
            
            // Mematikan auto-detect kiri/kanan bawaan agar tidak tertimpa
            hasSetDirection = true; 
        }

        void Update()
        {
            if (!hasSetDirection)
            {
                currentVelocity = initialVelocity;
                // Jika spawn di kanan layar (X > 0), paksa X negatif agar terbang ke kiri
                // Jika spawn di kiri layar (X <= 0), paksa X positif agar terbang ke kanan
                if (transform.position.x > 0)
                    currentVelocity.x = -Mathf.Abs(initialVelocity.x);
                else
                    currentVelocity.x = Mathf.Abs(initialVelocity.x);

                hasSetDirection = true;
            }
            // 1. Gradually change vertical velocity (Y) towards the target (pull up / curve)
            currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetYVelocity, pullUpAcceleration * Time.deltaTime);

            // 2. Apply velocity to position
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);

            // 3. Rotate the object to face the direction of velocity
            if (lookForward && currentVelocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

                // Membalikkan sprite (flip Y) agar tidak terbalik (upside down) saat menghadap kiri
                Vector3 scale = transform.localScale;
                scale.y = currentVelocity.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
                transform.localScale = scale;
            }
        }
    }
}
