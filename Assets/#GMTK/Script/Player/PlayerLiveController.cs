using UnityEngine;
using UnityEngine.Events;

namespace GMTK.Player
{
    public class PlayerLiveController : MonoBehaviour
    {
        [SerializeField] protected int intMaxHealth = 3;

        [SerializeField] protected UnityEvent onRespawn;
        [SerializeField] protected UnityEvent onGameOver;

        public int intCurrentHealth { get; protected set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            intCurrentHealth = intMaxHealth;
        }

        public void OnDeath()
        {
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
            onRespawn.AddListener(_onRespawn);
        }

        public void RemoveRespawnListener(UnityAction _onRespawn)
        {
            onRespawn.RemoveListener(_onRespawn);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}