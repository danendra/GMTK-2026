using UnityEngine;
using UnityEngine.UI;
using AudioSystem;
using System.Collections;
using MoreMountains.Feedbacks;

namespace GMTK.UI
{
    using Mode;

    public class UIHourglassController : MonoBehaviour
    {
        [SerializeField] protected Image imgHourglass;
        [SerializeField] protected Sprite[] arrSprites;
        [SerializeField] protected bool blnReverseAnimation = false;
        [SerializeField] protected float fltFlipDuration = 0.4f;
        [SerializeField] protected MMFeedbacks flipFeedbacks;
        [SerializeField] protected SoundData soundDataFlip;
        [SerializeField] protected SoundData sandSoundData;

        protected bool blnLastModeActive;
        protected bool blnIsFlipping = false;
        protected Coroutine coSandDrop;
        protected AudioSource sandAudioSource;

        protected virtual void Awake()
        {
            sandAudioSource = gameObject.AddComponent<AudioSource>();
            sandAudioSource.playOnAwake = false;
            sandAudioSource.spatialBlend = 0f;
            sandAudioSource.ignoreListenerPause = true;
        }

        protected virtual void Start()
        {
            if (ModeManager.instance != null)
            {
                blnLastModeActive = ModeManager.instance.isActive;
            }

            TriggerSandDropFeedback();
        }

        protected virtual void Update()
        {
            if (ModeManager.instance == null || imgHourglass == null || arrSprites == null || arrSprites.Length == 0)
            {
                return;
            }

            if (ModeManager.instance.isActive != blnLastModeActive)
            {
                blnLastModeActive = ModeManager.instance.isActive;
                TriggerFlip();
            }

            if (!blnIsFlipping)
            {
                float fltMaxTime = ModeManager.instance.GetMaxCountdown;
                if (fltMaxTime <= 0) return;

                float fltRatio = Mathf.Clamp01(ModeManager.instance.fltCountdown / fltMaxTime);
                float fltProgress = 1f - fltRatio;
                if (blnReverseAnimation)
                {
                    fltProgress = fltRatio;
                }

                int intIndex = Mathf.Clamp(Mathf.FloorToInt(fltProgress * arrSprites.Length), 0, arrSprites.Length - 1);
                imgHourglass.sprite = arrSprites[intIndex];
            }
        }

        protected void TriggerFlip()
        {
            blnIsFlipping = true;
            
            imgHourglass.sprite = arrSprites[arrSprites.Length - 1];

            imgHourglass.rectTransform.localEulerAngles = Vector3.zero;

            flipFeedbacks?.PlayFeedbacks();
            StartCoroutine(IEFinishFlip());

            if (soundDataFlip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.CreateSound()
                    .WithSoundData(soundDataFlip)
                    .WithRandomPitch()
                    .WithPosition(transform.position)
                    .Play();
            }

            StopSandSound();
            TriggerSandDropFeedback();
        }

        protected IEnumerator IEFinishFlip()
        {
            yield return new WaitForSeconds(Mathf.Max(0.01f, fltFlipDuration));

            imgHourglass.sprite = arrSprites[0];
            imgHourglass.rectTransform.localEulerAngles = Vector3.zero;
            blnIsFlipping = false;
        }

        protected void TriggerSandDropFeedback()
        {
            if (coSandDrop != null)
            {
                StopCoroutine(coSandDrop);
            }

            coSandDrop = StartCoroutine(IESandDropFeedback());
        }

        protected IEnumerator IESandDropFeedback()
        {
            if (sandSoundData != null)
            {
                sandAudioSource.clip = sandSoundData.Clip;
                sandAudioSource.outputAudioMixerGroup = sandSoundData.MixerGroup;
                sandAudioSource.loop = sandSoundData.Loop;
                sandAudioSource.volume = sandSoundData.volume;
                sandAudioSource.Play();
            }

            coSandDrop = null;
            yield break;
        }

        protected void StopSandSound()
        {
            if (sandAudioSource != null && sandAudioSource.isPlaying)
            {
                sandAudioSource.Stop();
            }
        }
    }
}
