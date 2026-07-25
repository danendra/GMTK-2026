using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GMTK.UI
{
    using Mode;

    public class UIHourglassController : MonoBehaviour
    {
        [SerializeField] protected Image imgHourglass;
        [SerializeField] protected Sprite[] arrSprites;
        [SerializeField] protected bool blnReverseAnimation = false;
        [SerializeField] protected float fltFlipDuration = 0.4f;

        protected bool blnLastModeActive;
        protected bool blnIsFlipping = false;

        protected virtual void Start()
        {
            if (ModeManager.instance != null)
            {
                blnLastModeActive = ModeManager.instance.isActive;
            }
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

            imgHourglass.rectTransform.DOKill();

            imgHourglass.rectTransform.DORotate(new Vector3(0f, 0f, 180f), fltFlipDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    imgHourglass.sprite = arrSprites[0];
                    imgHourglass.rectTransform.localEulerAngles = Vector3.zero;
                    blnIsFlipping = false;
                });
        }
    }
}
