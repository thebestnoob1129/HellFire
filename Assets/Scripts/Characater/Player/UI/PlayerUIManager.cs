using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager Instance { get; private set; }

        [Header("Network Join")]
        [SerializeField] private bool startGameAsClient;

        public PlayerUIHudManager playerHudManager;
        public PlayerUIPopUpManager playerPopUpManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);

            if (!playerHudManager) playerHudManager = GetComponentInChildren<PlayerUIHudManager>();
            if (!playerPopUpManager) playerPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                // SHUT DOWN AS HOST TO START AS CLIENT
                NetworkManager.Singleton.Shutdown();
                // START GAME AS CLIENT
                NetworkManager.Singleton.StartClient();
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
}