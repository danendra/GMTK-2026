using System;
using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace GMTK
{
    public class FeedbackManager : MonoBehaviour
    {
        public static FeedbackManager Instance { get; private set; }

        [Header("Player Feedbacks")]
        [SerializeField, Tooltip("Played when the player shoots.")]
        private MMF_Player _playerShoot;

        [SerializeField, Tooltip("Played when the player takes damage.")]
        private MMF_Player _playerDamaged;

        [SerializeField, Tooltip("Played when the player dies.")]
        private MMF_Player _playerDead;

        [Header("Enemy Feedbacks")]
        [SerializeField, Tooltip("Played when an enemy takes damage.")]
        private MMF_Player _enemyDamaged;

        [SerializeField, Tooltip("Played when an enemy dies.")]
        private MMF_Player _enemyDead;

        [SerializeField, Tooltip("Played when an enemy shoots.")]
        private MMF_Player _enemyShoot;

        [Header("Boss Feedbacks")]
        [SerializeField, Tooltip("Played when the boss takes damage.")]
        private MMF_Player _bossDamaged;

        [SerializeField, Tooltip("Played when the boss is destroyed.")]
        private MMF_Player _bossDestroy;

        [Header("System Feedbacks")]
        [SerializeField, Tooltip("Played before transitioning to the next level.")]
        private MMF_Player _switchLevel;

        // [Header("Camera Shake Settings")]
        // [SerializeField, Tooltip("Shake intensity when player is damaged.")]
        // private float _playerDamagedShakeIntensity = 0.1f;
        // [SerializeField, Tooltip("Shake duration when player is damaged.")]
        // private float _playerDamagedShakeDuration = 0.15f;

        // [SerializeField, Tooltip("Shake intensity when player dies.")]
        // private float _playerDeadShakeIntensity = 0.2f;
        // [SerializeField, Tooltip("Shake duration when player dies.")]
        // private float _playerDeadShakeDuration = 0.3f;

        // [SerializeField, Tooltip("Shake intensity when enemy dies.")]
        // private float _enemyDeadShakeIntensity = 0.05f;
        // [SerializeField, Tooltip("Shake duration when enemy dies.")]
        // private float _enemyDeadShakeDuration = 0.1f;

        // [SerializeField, Tooltip("Shake intensity when boss is destroyed.")]
        // private float _bossDestroyShakeIntensity = 0.3f;
        // [SerializeField, Tooltip("Shake duration when boss is destroyed.")]
        // private float _bossDestroyShakeDuration = 0.5f;

        // ══════════════════════════════════════════════
        // Unity Lifecycle
        // ══════════════════════════════════════════════

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // ══════════════════════════════════════════════
        // Player API
        // ══════════════════════════════════════════════

        public void PlayPlayerShoot(Transform target, SpriteRenderer sr = null)
        {
            if (_playerShoot != null)
                InjectTargetAndPlay(_playerShoot, target, sr);
        }

        public void PlayPlayerDamaged(Transform target, SpriteRenderer sr = null)
        {
            if (_playerDamaged != null)
                InjectTargetAndPlay(_playerDamaged, target, sr);

            // ShakeCamera(_playerDamagedShakeIntensity, _playerDamagedShakeDuration);
        }

        public void PlayPlayerDead(Transform target, Action onComplete = null, SpriteRenderer sr = null)
        {
            if (_playerDead != null)
            {
                InjectTargetAndPlay(_playerDead, target, sr);
                StartCoroutine(WaitForFeedbackComplete(_playerDead, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }

            // ShakeCamera(_playerDeadShakeIntensity, _playerDeadShakeDuration);
        }

        // ══════════════════════════════════════════════
        // Enemy API
        // ══════════════════════════════════════════════

        public void PlayEnemyDamaged(Transform target, SpriteRenderer sr = null)
        {
            if (_enemyDamaged != null)
                InjectTargetAndPlay(_enemyDamaged, target, sr);
        }

        public void PlayEnemyDead(Transform target, SpriteRenderer sr = null)
        {
            if (_enemyDead != null)
                InjectTargetAndPlay(_enemyDead, target, sr);

            // ShakeCamera(_enemyDeadShakeIntensity, _enemyDeadShakeDuration);
        }

        public void PlayEnemyShoot(Transform target, SpriteRenderer sr = null)
        {
            if (_enemyShoot != null)
                InjectTargetAndPlay(_enemyShoot, target, sr);
        }

        // ══════════════════════════════════════════════
        // Boss API
        // ══════════════════════════════════════════════

        public void PlayBossDamaged(Transform target, SpriteRenderer sr = null)
        {
            if (_bossDamaged != null)
                InjectTargetAndPlay(_bossDamaged, target, sr);
        }

        public void PlayBossDestroy(Transform target, Action onComplete = null, SpriteRenderer sr = null)
        {
            if (_bossDestroy != null)
            {
                InjectTargetAndPlay(_bossDestroy, target, sr);
                StartCoroutine(WaitForFeedbackComplete(_bossDestroy, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }

            // ShakeCamera(_bossDestroyShakeIntensity, _bossDestroyShakeDuration);
        }

        // ══════════════════════════════════════════════
        // System API
        // ══════════════════════════════════════════════

        public void PlaySwitchLevel(Action onComplete = null)
        {
            if (_switchLevel != null)
            {
                _switchLevel.PlayFeedbacks();
                StartCoroutine(WaitForFeedbackComplete(_switchLevel, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        // ══════════════════════════════════════════════
        // Internal Helpers
        // ══════════════════════════════════════════════

        private IEnumerator WaitForFeedbackComplete(MMF_Player player, Action onComplete)
        {
            yield return new WaitWhile(() => player.IsPlaying);
            onComplete?.Invoke();
        }

        // private void ShakeCamera(float intensity, float duration)
        // {
        //     if (CinemachineShake2D.Instance != null)
        //         CinemachineShake2D.Instance.Shake(intensity, duration);
        // }

        private void InjectTargetAndPlay(MMF_Player player, Transform target, SpriteRenderer sr = null)
        {
            if (sr == null && target != null)
                sr = target.GetComponentInChildren<SpriteRenderer>();

            var scaleFeedback = player.GetFeedbackOfType<MMF_Scale>();
            if (scaleFeedback != null)
                scaleFeedback.AnimateScaleTarget = target;

            var rotFeedback = player.GetFeedbackOfType<MMF_Rotation>();
            if (rotFeedback != null)
                rotFeedback.AnimateRotationTarget = target;

            var posFeedback = player.GetFeedbackOfType<MMF_Position>();
            if (posFeedback != null)
                posFeedback.AnimatePositionTarget = target.gameObject;

            var particleFeedbacks = player.GetFeedbacksOfType<MMF_ParticlesInstantiation>();
            if (particleFeedbacks != null)
            {
                foreach (var particle in particleFeedbacks)
                {
                    particle.PositionMode = MMF_ParticlesInstantiation.PositionModes.Script;
                    particle.NestParticles = false;
                }
            }

            if (sr != null)
            {
                var flickerFeedbacks = player.GetFeedbacksOfType<MMF_Flicker>();
                if (flickerFeedbacks != null)
                {
                    foreach (var flicker in flickerFeedbacks)
                        flicker.BoundRenderer = sr;
                }

                var srFeedbacks = player.GetFeedbacksOfType<MMF_SpriteRenderer>();
                if (srFeedbacks != null)
                {
                    foreach (var srFeedback in srFeedbacks)
                        srFeedback.BoundSpriteRenderer = sr;
                }
            }

            player.PlayFeedbacks(target.position);
        }

#if UNITY_EDITOR
        [ContextMenu("Generate Feedback Objects")]
        private void GenerateFeedbackObjects()
        {
            UnityEditor.Undo.RecordObject(this, "Generate Feedback Objects");

            _playerShoot = GetOrCreateFeedback("PlayerShoot", _playerShoot);
            _playerDamaged = GetOrCreateFeedback("PlayerDamaged", _playerDamaged);
            _playerDead = GetOrCreateFeedback("PlayerDead", _playerDead);

            _enemyDamaged = GetOrCreateFeedback("EnemyDamaged", _enemyDamaged);
            _enemyDead = GetOrCreateFeedback("EnemyDead", _enemyDead);
            _enemyShoot = GetOrCreateFeedback("EnemyShoot", _enemyShoot);

            _bossDamaged = GetOrCreateFeedback("BossDamaged", _bossDamaged);
            _bossDestroy = GetOrCreateFeedback("BossDestroy", _bossDestroy);

            _switchLevel = GetOrCreateFeedback("SwitchLevel", _switchLevel);

            UnityEditor.EditorUtility.SetDirty(this);
        }

        private MMF_Player GetOrCreateFeedback(string childName, MMF_Player existing)
        {
            if (existing != null) return existing;

            Transform child = transform.Find(childName);
            if (child == null)
            {
                GameObject go = new GameObject(childName);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                child = go.transform;
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Feedback Object");
            }

            MMF_Player mmfPlayer = child.GetComponent<MMF_Player>();
            if (mmfPlayer == null)
            {
                mmfPlayer = UnityEditor.Undo.AddComponent<MMF_Player>(child.gameObject);
            }

            return mmfPlayer;
        }
#endif
    }
}
