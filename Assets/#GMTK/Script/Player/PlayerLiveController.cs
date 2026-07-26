using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using System.Collections;

namespace GMTK.Player
{
    public class PlayerLiveController : MonoBehaviour
    {
        [SerializeField] protected int intMaxHealth = 3;

        [SerializeField] protected UnityEvent onRespawn;
        [SerializeField] protected UnityEvent onGameOver;
        [SerializeField] protected MMFeedbacks deathFeedbacks;

        public int intCurrentHealth { get; protected set; }
        
        protected PlayerBombController bombController;

        void Awake()
        {
            intCurrentHealth = intMaxHealth;

            bombController = GetComponentInChildren<PlayerBombController>();
        }

        public void OnDeath()
        {
            PlayDeathFeedback();

            intCurrentHealth--;

            if (intCurrentHealth < 1)
            {
                onGameOver.Invoke();
            }
            else
            {
                onRespawn.Invoke();
            }
        }
        public void AddRespawnListener(UnityAction _onRespawn)
        {
            if(bombController == null)
                bombController = GetComponentInChildren<PlayerBombController>();

            onRespawn.AddListener(_onRespawn);
            bombController.OnBombTrigger.AddListener(_onRespawn);
        }

        public void RemoveRespawnListener(UnityAction _onRespawn)
        {
            if(bombController == null)
                bombController = GetComponentInChildren<PlayerBombController>();

            onRespawn.RemoveListener(_onRespawn);
            bombController.OnBombTrigger.RemoveListener(_onRespawn);
        }

        void PlayDeathFeedback()
        {
            if (deathFeedbacks == null)
            {
                return;
            }

            MMFeedbacks feedbackInstance = Instantiate(deathFeedbacks, transform.position, transform.rotation);
            feedbackInstance.transform.SetParent(null);
            feedbackInstance.PlayFeedbacks();
            StartCoroutine(IECleanupDeathFeedback(feedbackInstance));
        }

        IEnumerator IECleanupDeathFeedback(MMFeedbacks feedbackInstance)
        {
            if (feedbackInstance == null)
            {
                yield break;
            }

            yield return new WaitForSeconds(2f);

            if (feedbackInstance != null)
            {
                Destroy(feedbackInstance.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}