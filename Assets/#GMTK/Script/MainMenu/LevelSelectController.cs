using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GMTK.MainMenu
{
    [Serializable]
    public class LevelData
    {
        public string sceneName;
        public string levelName;
        public bool unlocked;
        public Button levelButton;
        public GameObject lockMark;
    }

    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] private LevelData[] levels;

        private void Start()
        {
            SetupLevels();
        }

        private void SetupLevels()
        {
            for (int i = 0; i < levels.Length; i++)
            {
                LevelData level = levels[i];

                // Nyalakan lockmark hanya ketika level terkunci.
                if (level.lockMark != null)
                    level.lockMark.SetActive(!level.unlocked);

                if (level.levelButton != null)
                {
                    // Button tidak bisa ditekan ketika level terkunci.
                    level.levelButton.interactable = level.unlocked;

                    // Simpan referensi lokal agar closure menangkap level yang benar.
                    LevelData captured = level;
                    level.levelButton.onClick.RemoveAllListeners();
                    level.levelButton.onClick.AddListener(() => LoadLevel(captured));
                }
            }
        }

        private void LoadLevel(LevelData level)
        {
            if (!level.unlocked)
                return;

            if (string.IsNullOrEmpty(level.sceneName))
            {
                Debug.LogWarning($"Scene name kosong untuk level '{level.levelName}'.", this);
                return;
            }

            SceneManager.LoadScene(level.sceneName);
        }
    }
}
