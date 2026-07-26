using System.Collections;
using UnityEngine;

namespace GMTK.Utils
{
    public class ObjectShaker : MonoBehaviour
    {
        [Header("Default Settings")]
        [SerializeField, Tooltip("The transform to shake. If empty, shakes this GameObject.")]
        private Transform targetTransform;

        [SerializeField, Tooltip("If true, automatically plays the shake on Start.")]
        private bool playOnStart = false;

        [SerializeField, Tooltip("If true, automatically plays the shake on Enable.")]
        private bool playOnEnable = false;

        [SerializeField, Tooltip("If true, the Play On Enable shake will only trigger the very first time this object is enabled.")]
        private bool playOnEnableOnlyOnce = true;

        [SerializeField, Tooltip("Delay in seconds before the shake starts.")]
        private float initialDelay = 0f;

        [SerializeField, Tooltip("How many times the shake should loop when triggered (0 means infinite).")]
        private int playCount = 1;

        [SerializeField, Tooltip("Delay in seconds between repeated shakes.")]
        private float delayBetweenShakes = 0f;

        [SerializeField, Tooltip("Duration of the shake in seconds.")]
        private float duration = 0.2f;

        [SerializeField, Tooltip("Maximum intensity/distance of the shake.")]
        private float intensity = 0.5f;

        [Header("Customization")]
        [Tooltip("Direction multiplier for the shake. Set to (1, 0, 0) to only shake horizontally, or (0, 1, 0) for vertical.")]
        [SerializeField] private Vector3 shakeDirection = new Vector3(1f, 1f, 0f);

        [Tooltip("If true, the shake intensity will gradually decrease over time.")]
        [SerializeField] private bool fadeOut = true;

        [Tooltip("If true, uses Perlin Noise for a smooth continuous shake. If false, uses random noise for a rough, sudden shake (good for hit impacts).")]
        [SerializeField] private bool smoothShake = false;

        [Tooltip("Speed of the smooth shake (only applied if smoothShake is true).")]
        [SerializeField] private float smoothShakeSpeed = 50f;

        private Coroutine shakeCoroutine;
        private Vector3 initialPosition;
        private bool hasPlayedOnEnable = false;

        private void Awake()
        {
            if (targetTransform == null)
            {
                targetTransform = transform;
            }
            initialPosition = targetTransform.localPosition;
        }

        private void OnEnable()
        {
            if (playOnEnable)
            {
                if (playOnEnableOnlyOnce && hasPlayedOnEnable) return;

                PlayShake(duration, intensity, initialDelay);
                hasPlayedOnEnable = true;
            }
        }

        private void Start()
        {
            if (playOnStart)
            {
                PlayShake(duration, intensity, initialDelay);
            }
        }

        private void OnDisable()
        {
            StopShake();
        }

        /// <summary>
        /// Starts shaking using the default settings configured in the inspector.
        /// </summary>
        [ContextMenu("Test Shake")]
        public void TesShake()
        {
            PlayShake(duration, intensity, initialDelay);
        }

        /// <summary>
        /// Starts shaking with custom duration, intensity, and optional delay.
        /// </summary>
        public void PlayShake(float customDuration, float customIntensity, float customDelay = 0f)
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }
            else
            {
                // Only store initial position if we are not currently shaking
                initialPosition = targetTransform.localPosition;
            }

            shakeCoroutine = StartCoroutine(ShakeRoutine(customDuration, customIntensity, customDelay));
        }

        /// <summary>
        /// Immediately stops the shake and returns the object to its original position.
        /// </summary>
        public void StopShake()
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                if (targetTransform != null) targetTransform.localPosition = initialPosition;
                shakeCoroutine = null;
            }
        }

        private IEnumerator ShakeRoutine(float totalDuration, float maxIntensity, float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
                // Update initial position right before shaking in case the object moved during the delay
                if (targetTransform != null) initialPosition = targetTransform.localPosition;
            }

            int currentLoop = 0;

            while (playCount <= 0 || currentLoop < playCount)
            {
                float elapsed = 0f;
                Vector3 startPos = initialPosition;

                // Random offset for Perlin Noise to ensure a different pattern each time
                float randomSeed = Random.Range(0f, 100f);

                while (elapsed < totalDuration)
                {
                    elapsed += Time.deltaTime;

                    float currentIntensity = fadeOut
                        ? Mathf.Lerp(maxIntensity, 0f, elapsed / totalDuration)
                        : maxIntensity;

                    Vector3 offset = Vector3.zero;

                    if (smoothShake)
                    {
                        // Smooth shaking using Perlin Noise
                        float x = (Mathf.PerlinNoise(Time.time * smoothShakeSpeed + randomSeed, 0f) - 0.5f) * 2f;
                        float y = (Mathf.PerlinNoise(0f, Time.time * smoothShakeSpeed + randomSeed) - 0.5f) * 2f;
                        float z = (Mathf.PerlinNoise(Time.time * smoothShakeSpeed + randomSeed, Time.time * smoothShakeSpeed + randomSeed) - 0.5f) * 2f;

                        offset = new Vector3(x, y, z);
                    }
                    else
                    {
                        // Rough shaking using Random.Range (good for impacts)
                        offset = new Vector3(
                            Random.Range(-1f, 1f),
                            Random.Range(-1f, 1f),
                            Random.Range(-1f, 1f)
                        );
                    }

                    // Apply directional constraints
                    offset.x *= shakeDirection.x;
                    offset.y *= shakeDirection.y;
                    offset.z *= shakeDirection.z;

                    if (targetTransform != null) targetTransform.localPosition = startPos + (offset * currentIntensity);

                    yield return null;
                }

                // Snap back to the original position once the shake is complete
                if (targetTransform != null) targetTransform.localPosition = startPos;
                
                currentLoop++;

                if ((playCount <= 0 || currentLoop < playCount) && delayBetweenShakes > 0f)
                {
                    yield return new WaitForSeconds(delayBetweenShakes);
                }
            }

            shakeCoroutine = null;
        }
    }
}
