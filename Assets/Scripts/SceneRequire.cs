using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRequire: MonoBehaviour
{
    
    public string[] requiredScenes;
    public PlayerManager[] players;


    private void Awake()
    {
        if (GameObject.FindWithTag("MainCamera")) Destroy(GameObject.FindWithTag("MainCamera"));

        foreach (var sceneName in requiredScenes)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
        players = FindObjectsByType<PlayerManager>(FindObjectsSortMode.InstanceID);
    }
}
