using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("Layers")]
    [Tooltip("Fill with A, B, C (list order doesn't have to be tidy if arrangeOnStart is enabled). Leave empty to grab all children automatically.")]
    [SerializeField] private List<Transform> layers = new List<Transform>();

    [Header("Scroll")]
    [Tooltip("Downward speed in world units per second.")]
    [SerializeField] private float speed = 2f;

    [Tooltip("Default transition duration in seconds used by SetSpeed when no duration is given.")]
    [SerializeField] private float defaultSpeedBlendDuration = 0.5f;

    [Header("Setup")]
    [Tooltip("Rearrange the layers into a perfect stack on Start, clearing any manual positioning errors made in the editor.")]
    [SerializeField] private bool arrangeOnStart = true;

    [Tooltip("Reference camera used to determine the 'off screen' boundary. Empty = Camera.main.")]
    [SerializeField] private Camera targetCamera;

    private float spriteHeight;
    private float totalHeight;

    // Speed transition state. blendDuration <= 0 means no transition is running.
    private float blendFrom;
    private float blendTo;
    private float blendDuration;
    private float blendElapsed;

    /// Current scroll speed, including any transition currently in progress.
    public float Speed => speed;

    /// Speed the controller is heading towards (equals Speed when nothing is blending).
    public float TargetSpeed => blendDuration > 0f ? blendTo : speed;

    /// Changes the speed immediately, cancelling any transition in progress.
    public void SetSpeedInstant(float newSpeed)
    {
        speed = newSpeed;
        blendDuration = 0f;
    }

    /// Blends the speed over defaultSpeedBlendDuration seconds.
    public void SetSpeed(float newSpeed) => SetSpeed(newSpeed, defaultSpeedBlendDuration);

    /// Blends the speed over the given duration. A duration <= 0 applies it instantly.
    public void SetSpeed(float newSpeed, float duration)
    {
        if (duration <= 0f)
        {
            SetSpeedInstant(newSpeed);
            return;
        }

        // Start from the current speed so overlapping calls chain smoothly.
        blendFrom = speed;
        blendTo = newSpeed;
        blendDuration = duration;
        blendElapsed = 0f;
    }

    /// Stops an ongoing transition and keeps the speed at its current value.
    public void CancelSpeedBlend() => blendDuration = 0f;

    private void UpdateSpeedBlend()
    {
        if (blendDuration <= 0f)
            return;

        blendElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(blendElapsed / blendDuration);
        speed = Mathf.Lerp(blendFrom, blendTo, Mathf.SmoothStep(0f, 1f, t));

        if (t >= 1f)
        {
            speed = blendTo;
            blendDuration = 0f;
        }
    }

    private void Start()
    {
        // Use children as layers if the list is empty.
        if (layers.Count == 0)
        {
            foreach (Transform child in transform)
                layers.Add(child);
        }

        if (layers.Count < 2)
        {
            Debug.LogError($"{name}: needs at least 2 layers to loop without gaps (3 is ideal).", this);
            enabled = false;
            return;
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
        {
            Debug.LogError($"{name}: no camera found. Set targetCamera or tag a camera as MainCamera.", this);
            enabled = false;
            return;
        }

        // Height of a single sprite from the renderer bounds (assumes all layers share the same height).
        var sr = layers[0].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"{name}: layer '{layers[0].name}' has no SpriteRenderer.", this);
            enabled = false;
            return;
        }

        spriteHeight = sr.bounds.size.y;
        totalHeight = spriteHeight * layers.Count;

        if (arrangeOnStart)
            ArrangeStack();

        WarnIfViewportTooTall();
    }

    /// Stacks every layer directly above layers[0], spaced exactly one sprite height apart.
    /// layers[0] acts as the anchor (its position is kept), the rest are realigned.
    private void ArrangeStack()
    {
        Vector3 basePos = layers[0].position;
        for (int i = 0; i < layers.Count; i++)
        {
            Vector3 p = layers[i].position;
            p.x = basePos.x;
            p.y = basePos.y + i * spriteHeight;
            layers[i].position = p;
        }
    }

    private void LateUpdate()
    {
        UpdateSpeedBlend();

        float move = speed * Time.deltaTime;

        // Reset line: the center Y at which the sprite's TOP edge exactly touches the bottom of the screen.
        // Once the center drops below this line, the whole sprite has passed off the bottom of the screen.
        float camBottom = targetCamera.transform.position.y - targetCamera.orthographicSize;
        float resetLine = camBottom - spriteHeight * 0.5f;

        for (int i = 0; i < layers.Count; i++)
        {
            Vector3 p = layers[i].position;
            p.y -= move;

            // while (not if) to stay safe when scroll speed is very high / on frame drops.
            while (p.y < resetLine)
                p.y += totalHeight;

            layers[i].position = p;
        }
    }

    private void WarnIfViewportTooTall()
    {
        float viewHeight = targetCamera.orthographicSize * 2f;
        float coverWhileTransit = (layers.Count - 1) * spriteHeight;
        if (coverWhileTransit < viewHeight)
        {
            Debug.LogWarning(
                $"{name}: ({layers.Count - 1}) x spriteHeight ({coverWhileTransit:F2}) < view height ({viewHeight:F2}). " +
                "A gap may appear while one layer is in transit. Use a taller sprite or add more layers.", this);
        }
    }
}