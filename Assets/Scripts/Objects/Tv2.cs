using UnityEngine;
using UnityEngine.SceneManagement;

namespace CFS
{
    public class Tv2 : MonoBehaviour, IInteractable
    {

        [Header("TV Screen")]
        [SerializeField] private Camera tvCamera;
        [SerializeField] private Renderer screen;
        public LayerMask playerLayer;
        private RenderTexture renderTexture;
        public Material tvMaterial;
        public Material staticMaterial;

        [Header("Menu")]
        public GameObject uiObject;
        private Material tvMat;

        public void Interact()
        {
            uiObject.SetActive(true);
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
}