using UnityEngine;

namespace GMTK.Enemy
{
    using Player;
    public enum LASER_STATE { Idle, Telegraph, Firing }

    public class EnemyLaserSpawnerController : MonoBehaviour
    {        
        [SerializeField] protected LineRenderer lineRenderer;
        [SerializeField] protected Vector2 direction = Vector2.up;
        [SerializeField] protected float laserLength = 20f;        
        [SerializeField] protected float telegraphWidth = 0.05f;
        [SerializeField] protected float fireWidth = 0.3f;
        [SerializeField] protected Color telegraphColor = Color.red;
        [SerializeField] protected Color fireColor = Color.white;
        [SerializeField] protected LayerMask hitLayers;

        public LASER_STATE CurrentState { get; protected set; } = LASER_STATE.Idle;

        protected float telegraphDuration = 1f;
        protected float fireDuration = 2f;
        protected float stateTimer;
        protected PlayerLiveController playerLive;

        void Awake()
        {
            playerLive = FindAnyObjectByType<PlayerLiveController>();
        }

        void OnEnable()
        {
            if(!playerLive)
                Awake();

            playerLive?.AddRespawnListener(Reset);
        }

        void OnDisable()
        {
            if(!playerLive)
                Awake();

            playerLive?.RemoveRespawnListener(Reset);
        }

        private void Update()
        {
            if (CurrentState == LASER_STATE.Idle) return;
            
            stateTimer += Time.deltaTime;

            switch (CurrentState)
            {
                case LASER_STATE.Telegraph:
                    UpdateBeam();
                    if (stateTimer >= telegraphDuration)
                    {
                        CurrentState = LASER_STATE.Firing;
                        stateTimer = 0f;
                        SetLineVisual(fireWidth, fireColor);
                    }
                    break;

                case LASER_STATE.Firing:
                    UpdateBeam();
                    if (stateTimer >= fireDuration + telegraphDuration)
                    {
                        CurrentState = LASER_STATE.Idle;
                        lineRenderer.enabled = false;
                    }
                    break;
            }
        }

        private void UpdateBeam()
        {
            Vector2 origin = transform.position;
            Vector2 worldDirection = transform.TransformDirection(direction);

            RaycastHit2D hit = Physics2D.Raycast(origin, worldDirection, laserLength, hitLayers);

            float actualLength = hit.collider != null ? hit.distance : laserLength;

            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + worldDirection * actualLength);

            if (CurrentState == LASER_STATE.Firing && hit.collider != null)
            {
                PlayerLiveController _playerLife = hit.collider.gameObject.GetComponent<PlayerLiveController>();

                if (_playerLife)
                {
                    _playerLife.OnDeath();
                }
            }
        }

        private void SetLineVisual(float width, Color color)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void Spawn(float _fltDurationFire = 2.0f, float _fltDurationTelegraph = 1.0f)
        {
            if (CurrentState != LASER_STATE.Idle) return;
            CurrentState = LASER_STATE.Telegraph;
            
            stateTimer = 0f;
            fireDuration = _fltDurationFire;
            telegraphDuration = _fltDurationTelegraph;
            
            SetLineVisual(telegraphWidth, telegraphColor);
            lineRenderer.enabled = true;
            UpdateBeam();
        }

        public void Reset()
        {
            CurrentState = LASER_STATE.Idle;
            lineRenderer.enabled = false;
        }
    }
}
