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

        public TextMeshProUGUI difficultyLabel;
        public TextMeshProUGUI lockedLabel;


        private void Start()
        {
            // Redo load level data automatically
        }

        public void LoadScene()
        {
            StartCoroutine(nameof(LoadSceneCoroutine));
        }

        public IEnumerator LoadSceneCoroutine()
        {
            Debug.Log(SceneManager.GetSceneByName(sceneName).name + " is loading", gameObject);
            var load = SceneManager.LoadSceneAsync(sceneName);

            yield return load;
        }

    }
} 