using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK.UI
{
    using Player;

    public class UIBombController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Image imgBombFill;
        [SerializeField] protected Image imgBombIcon;

        [Header("Icon Settings")]
        [SerializeField] protected Color colorCharging = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] protected Color colorReady = Color.white;

        private PlayerBombController playerBomb;

        private void Start()
        {
            StartCoroutine(FindAndLinkBomb());
        }

        private IEnumerator FindAndLinkBomb()
        {
            playerBomb = FindObjectOfType<PlayerBombController>();
            while (playerBomb == null)
            {
                yield return null;
                playerBomb = FindObjectOfType<PlayerBombController>();
            }

            Refresh();
        }

        private void Update()
        {
            if (playerBomb == null) return;
            Refresh();
        }

        private void Refresh()
        {
            if (playerBomb == null) return;

            if (imgBombFill != null)
            {
                if (imgBombFill.type != Image.Type.Filled)
                {
                    imgBombFill.type = Image.Type.Filled;
                    imgBombFill.fillMethod = Image.FillMethod.Horizontal;
                    imgBombFill.fillOrigin = (int)Image.OriginHorizontal.Left;
                }

                float ratio = playerBomb.FltMaxCharge > 0 
                    ? playerBomb.FltCurrentCharge / playerBomb.FltMaxCharge 
                    : 0f;

                imgBombFill.fillAmount = Mathf.Clamp01(ratio);
            }

            if (imgBombIcon != null)
            {
                imgBombIcon.color = playerBomb.IsReady ? colorReady : colorCharging;
            }
        }
    }
}
