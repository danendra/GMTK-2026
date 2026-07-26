using UnityEngine;

using Anoa.Module;
using DG.Tweening;

namespace GMTK.Enemy
{
    public class RandomMovementController : MonoBehaviour
    {
        [SerializeField] protected Vector2 posCenter;
        [SerializeField] protected float fltRadius = 5.0f;
        [SerializeField] protected float fltMoveDuration = 0.2f;
        [SerializeField] protected float fltDelay = 2.0f;

        protected CooldownModule cooldown;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cooldown = new CooldownModule(fltDelay, false);
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown.IsReady)
            {
                RandomMove();
            }
        }

        public void RandomMove()
        {
            transform.DOMove(posCenter + Random.insideUnitCircle * fltRadius, fltMoveDuration);

            cooldown.Use();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.darkCyan;

            Gizmos.DrawWireSphere(posCenter, fltRadius);
        }
    }
}