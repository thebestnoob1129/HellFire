using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{


    public void Play()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
    public void Showcase(){}
    public void Settings(){}

    public void Exit()
    {
#if UNITY_EDITOR
        // Exit Play Mode in the Editor
        EditorApplication.isPlaying = false;
#else
        // Quit the application in a built game
        Application.Quit();
#endif
    }

}
