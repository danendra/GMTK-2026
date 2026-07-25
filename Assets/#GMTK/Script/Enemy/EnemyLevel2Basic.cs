using UnityEngine;
using System.Reflection;
using DG.Tweening;

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

        [Header("Visual & DOTween Settings")]
        [Tooltip("Transform dari objek Sprite (anak dari GameObject ini). Kosongkan jika ingin memakai cara Flip Scale lama.")]
        [SerializeField] private Transform spriteTransform;

        [Tooltip("Rotasi lokal (Euler) untuk Sprite saat terbang ke KANAN.")]
        [SerializeField] private Vector3 rightLocalRotation = Vector3.zero;

        [Tooltip("Rotasi lokal (Euler) untuk Sprite saat terbang ke KIRI. (Ubah sumbu X atau Y menjadi 180 jika pesawat terbalik).")]
        [SerializeField] private Vector3 leftLocalRotation = new Vector3(180f, 0f, 0f);

        [Tooltip("Durasi animasi putaran pesawat (Barrel Roll).")]
        [SerializeField] private float flipDuration = 0.4f;

        [Header("Boundary Settings")]
        [Tooltip("Batas posisi X di mana pesawat akan dimatikan (kembali ke pool).")]
        [SerializeField] private float disableXBoundary = 15f; 

        private Vector2 currentVelocity;
        private bool hasSetDirection;

        // --- Hack to rotate bullet spawners without modifying their script ---
        private EnemyBulletSpawnerController[] cachedSpawners;
        private Vector2[] originalSpawnerDirs;
        private FieldInfo dirField;
        private Quaternion lastRotation;

        private bool isFacingLeft = false;
        private bool isFirstFrame = true;

        void Awake()
        {
            cachedSpawners = GetComponentsInChildren<EnemyBulletSpawnerController>();
            originalSpawnerDirs = new Vector2[cachedSpawners.Length];
            
            // Ambil akses ke field 'direction' yang di-protect menggunakan Reflection
            dirField = typeof(EnemyBulletSpawnerController).GetField("direction", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (dirField != null)
            {
                for (int i = 0; i < cachedSpawners.Length; i++)
                {
                    // Simpan arah bawaan (original) dari Inspector
                    originalSpawnerDirs[i] = (Vector2)dirField.GetValue(cachedSpawners[i]);
                }
            }
        }

        void OnEnable()
        {
            hasSetDirection = false;
            isFirstFrame = true;
        }

        public void SetTargetPoint(Vector2 targetPos, bool isStraightLine)
        {
            // Menghitung arah dari posisi sekarang ke target
            Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
            
            // Kecepatan total pesawat
            float totalSpeed = initialVelocity.magnitude;
            
            if (isStraightLine)
            {
                // Timpa seluruh kecepatan agar meluncur LURUS (flat) ke target
                initialVelocity = direction * totalSpeed;
                currentVelocity = initialVelocity;
                
                // Samakan target Y dengan current Y agar tidak ada gaya tarik/melengkung sama sekali
                targetYVelocity = currentVelocity.y;
            }
            else
            {
                // Tetap melengkung (Swoop): X mengarah target, tapi Y mulai dari setelan awal dan melengkung ke target
                float currentSpeedX = Mathf.Abs(initialVelocity.x);
                initialVelocity.x = direction.x > 0 ? currentSpeedX : -currentSpeedX;
                
                targetYVelocity = direction.y * totalSpeed;
                currentVelocity = initialVelocity;
            }
            
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

                initialVelocity.x = currentVelocity.x; // Sync back
                
                hasSetDirection = true;
            }
            // 1. Gradually change vertical velocity (Y) towards the target (pull up / curve)
            currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetYVelocity, pullUpAcceleration * Time.deltaTime);

            // 2. Apply velocity to position
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);

            // 3. Rotate the object to face the direction of initial velocity / target
            if (lookForward && initialVelocity != Vector2.zero)
            {
                // Menghitung sudut berdasarkan posisi awal/target (initialVelocity), BUKAN currentVelocity yang berubah-ubah
                float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

                // Membalikkan sprite menggunakan DOTween (jika disetup) atau Fallback Scale
                bool movingLeft = initialVelocity.x < 0;

                if (spriteTransform != null)
                {
                    if (movingLeft != isFacingLeft || isFirstFrame)
                    {
                        isFacingLeft = movingLeft;
                        Vector3 targetRot = isFacingLeft ? leftLocalRotation : rightLocalRotation;
                        
                        spriteTransform.DOKill(); // Hentikan animasi lama
                        if (isFirstFrame)
                        {
                            spriteTransform.localEulerAngles = targetRot; // Instant di frame pertama
                        }
                        else
                        {
                            spriteTransform.DOLocalRotate(targetRot, flipDuration, RotateMode.Fast); // Animasi Barrel Roll
                        }
                    }
                }
                else
                {
                    // Fallback jadul jika spriteTransform kosong
                    Vector3 scale = transform.localScale;
                    scale.y = movingLeft ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
                    transform.localScale = scale;
                }
                
                isFirstFrame = false;
            }

            UpdateSpawnersDirection();
        }

        private void UpdateSpawnersDirection()
        {
            if (dirField == null || cachedSpawners == null || cachedSpawners.Length == 0) return;
            if (transform.rotation == lastRotation) return; // Hanya jalankan kode berat jika rotasi benar-benar berubah
            
            lastRotation = transform.rotation;
            
            for (int i = 0; i < cachedSpawners.Length; i++)
            {
                // Putar arah tembak bawaan sesuai dengan rotasi pesawat saat ini
                Vector2 rotatedDir = transform.rotation * originalSpawnerDirs[i];
                
                // Timpa nilai direction di script milik temanmu secara diam-diam
                dirField.SetValue(cachedSpawners[i], rotatedDir);
            }
        }
    }
}
