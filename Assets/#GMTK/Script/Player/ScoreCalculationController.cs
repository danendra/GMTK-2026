using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK.Player
{
    public class ScoreCalculationController : MonoBehaviour
    {
        public static ScoreCalculationController instance {get; protected set;}
        public int intHighScore {get; protected set;}
        public float intCurrentScore {get; protected set;}

        protected PlayerLiveController playerLive;

        void Awake()
        {
            instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            intHighScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name+"Hi Score", 0);
            intCurrentScore = 0;

            do
            {
                playerLive = FindAnyObjectByType<PlayerLiveController>();

                yield return null;
            }
            while (playerLive == null);
        }

        public void AddScore(float _intScore)
        {
            intCurrentScore += _intScore;

            if(intCurrentScore > intHighScore)
            {
                intHighScore = Mathf.RoundToInt(intCurrentScore);
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name+"Hi Score", intHighScore);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (playerLive != null)
            {
                AddScore(Time.deltaTime);
            }
        }
    }
}