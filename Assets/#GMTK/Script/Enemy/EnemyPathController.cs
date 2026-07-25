using UnityEngine;
using DG.Tweening;

namespace GMTK.Enemy
{
    public class EnemyPathController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Durasi waktu (dalam detik) yang dibutuhkan musuh untuk menyelesaikan seluruh jalur path.")]
        [SerializeField] private float duration = 5f;
        
        [Tooltip("Jenis kemudahan pergerakan (Easing) DOTween.")]
        [SerializeField] private Ease easeType = Ease.Linear;
        
        [Tooltip("Tipe kurva pergerakan (CatmullRom untuk melengkung mulus, Linear untuk garis patah-patah).")]
        [SerializeField] private PathType pathType = PathType.CatmullRom;
        
        [Tooltip("Mode pergerakan path 2D.")]
        [SerializeField] private PathMode pathMode = PathMode.Sidescroller2D;

        private bool dontDeactivateAtEnd = false;

        [Header("Rotation Settings")]
        [Tooltip("Aktifkan agar pesawat otomatis berputar menghadap arah geraknya.")]
        [SerializeField] private bool lookForward = true;
        
        [Tooltip("Offset rotasi default (sesuaikan jika sprite pesawat awal tidak menghadap kanan).")]
        [SerializeField] private float rotationOffset = 0f;

        private Tween pathTween;
        private Vector3 lastPosition;
        private Vector3 originalScale;

        void Awake()
        {
            originalScale = transform.localScale;
        }

        public void SetPathSettings(float duration, Ease easeType, PathType pathType, PathMode pathMode)
        {
            this.duration = duration;
            this.easeType = easeType;
            this.pathType = pathType;
            this.pathMode = pathMode;
        }

        public void SetDontDeactivate(bool value)
        {
            this.dontDeactivateAtEnd = value;
        }

        public void SetTargetPoint(Vector2 targetPos)
        {
            Vector3[] path = new Vector3[] { transform.position, (Vector3)targetPos };
            StartPath(path);
        }

        public void SetPath(Vector2[] pathPoints)
        {
            if (pathPoints == null || pathPoints.Length == 0) return;

            // Buat array waypoints termasuk posisi awal musuh
            Vector3[] path = new Vector3[pathPoints.Length + 1];
            path[0] = transform.position;
            for (int i = 0; i < pathPoints.Length; i++)
            {
                path[i + 1] = (Vector3)pathPoints[i];
            }

            StartPath(path);
        }

        private void StartPath(Vector3[] waypoints)
        {
            // Matikan tween aktif sebelumnya (sangat penting untuk object pooling)
            pathTween?.Kill();

            lastPosition = transform.position;

            // Jalankan pergerakan sepanjang path
            var pathTweenCore = transform.DOPath(waypoints, duration, pathType, pathMode)
                .SetEase(easeType);

            if (!dontDeactivateAtEnd)
            {
                pathTweenCore.OnComplete(() => gameObject.SetActive(false));
            }

            pathTween = pathTweenCore;
        }

        void Update()
        {
            // Rotasikan dan flip objek menghadap arah jalurnya
            if (lookForward && transform.position != lastPosition)
            {
                Vector3 moveDirection = (transform.position - lastPosition).normalized;
                if (moveDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

                    // Membalikkan sprite secara vertikal (flip Y) agar tidak terbalik (upside down) saat menghadap kiri
                    Vector3 scale = originalScale;
                    scale.y = moveDirection.x < 0 ? -Mathf.Abs(originalScale.y) : Mathf.Abs(originalScale.y);
                    transform.localScale = scale;
                }
            }
            lastPosition = transform.position;
        }

        void OnDisable()
        {
            // Hentikan tween ketika kembali ke pool
            pathTween?.Kill();
        }

        void OnDestroy()
        {
            pathTween?.Kill();
        }
    }
}
