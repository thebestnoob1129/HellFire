using CFS;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRequire: MonoBehaviour
{
    [Header("Scene Info")]
    public PlayerManager[] players;
    public string[] requiredScenes;

    [Header("Scene Settings")]
    private Camera cam;
    public bool isDark;

    private void Awake()
    {
        foreach (var sceneName in requiredScenes)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
        players = FindObjectsByType<PlayerManager>(FindObjectsSortMode.InstanceID);
        cam = PlayerCamera.Instance.cameraObject;
    }

    private void Start()
    {
        if (isDark)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
        }
    }


}
