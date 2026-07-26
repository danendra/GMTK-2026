using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace GMTK.Player
{
    public class ScoreCalculationController : MonoBehaviour
    {
        public static ScoreCalculationController instance {get; protected set;}
        public int intHighScore {get; protected set;}
        public float intCurrentScore {get; protected set;}

        protected PlayerLiveController playerLive;
        [SerializeField] protected TMP_Text txtHighscore;
        [SerializeField] protected TMP_Text txtScore;

        void Awake()
        {
            instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            intHighScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name+"Hi Score", 0);
            intCurrentScore = 0;
            RefreshScoreTexts();

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

            RefreshScoreTexts();
        }

        protected void RefreshScoreTexts()
        {
            if (txtScore != null)
            {
                txtScore.text = Mathf.RoundToInt(intCurrentScore).ToString();
            }

            if (txtHighscore != null)
            {
                txtHighscore.text = intHighScore.ToString();
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