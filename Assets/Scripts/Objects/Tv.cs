using UnityEngine;
using UnityEngine.SceneManagement;

public class Tv : MonoBehaviour, IInteractable
{
    public string sceneName = string.Empty;
    
    [SerializeField] private Camera tvCamera;
    [SerializeField] private Renderer screen;

    public LayerMask playerLayer;

    private RenderTexture renderTexture;

    public Material tvMaterial;
    public Material staticMaterial;

    private Material tvMat;
    public void Interact()
    {
        // If player has scene unlocked , load scene
        Debug.Log("Interacting with TV to load scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        // Render Texture
        renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        tvCamera.targetTexture = renderTexture;
        // Material
        tvMat = new Material(tvMaterial)
        {
            mainTexture = renderTexture
        };

        screen.material = staticMaterial;

    }

    private void FixedUpdate()
    {
        screen.material = Physics.CheckSphere(transform.position, 3f, playerLayer) ? tvMat : staticMaterial;
    }


}
