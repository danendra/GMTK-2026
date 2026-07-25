using UnityEngine;

using GMTK.Player;

namespace GMTK.Enemy
{
    public enum LASER_STATE { Idle, Telegraph, Firing }

    public class EnemyLaserSpawnerController : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] protected Vector2 direction = Vector2.up;
        [SerializeField] private float laserLength = 20f;
        [SerializeField] private float telegraphDuration = 1f;
        [SerializeField] private float fireDuration = 2f;
        [SerializeField] private float telegraphWidth = 0.05f;
        [SerializeField] private float fireWidth = 0.3f;
        [SerializeField] private Color telegraphColor = Color.red;
        [SerializeField] private Color fireColor = Color.white;
        [SerializeField] private LayerMask hitLayers;
        public LASER_STATE CurrentState { get; private set; } = LASER_STATE.Idle;

        private float stateTimer;

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
                    if (stateTimer >= fireDuration)
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
        }

        private void SetLineVisual(float width, Color color)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void Spawn()
        {
            if (CurrentState != LASER_STATE.Idle) return;
            CurrentState = LASER_STATE.Telegraph;
            stateTimer = 0f;
            SetLineVisual(telegraphWidth, telegraphColor);
            lineRenderer.enabled = true;
            UpdateBeam();
        }
    }
}
