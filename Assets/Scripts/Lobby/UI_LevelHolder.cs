using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CFS
{
    public class UI_LevelHolder : MonoBehaviour
    {
        public string sceneName;
        public Button levelButton;

        [Header("Level Info")]
        public CharacterSaveData.LevelData levelData;

        public TextMeshProUGUI difficultyLabel;
        public TextMeshProUGUI lockedLabel;


        private void Start()
        {
            // Doesn't Make Complete sense
            if (levelData.GetLevelBySceneName(sceneName) != null)
            {
                lockedLabel.text = levelData.isCompleted ? ">" : "x";
            }
        }

        public void LoadScene()
        {
            StartCoroutine(nameof(LoadSceneCoroutine));
            if (!levelData.isUnlocked) return;
        }

        public IEnumerator LoadSceneCoroutine()
        {
            Debug.Log(SceneManager.GetSceneByName(sceneName).name + " is loading", gameObject);
            var load = SceneManager.LoadSceneAsync(sceneName);

            yield return load;
        }

    }
} 