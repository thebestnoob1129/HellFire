using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDHandler hudHandler;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject jumpScareUI;


    public PlayerStats playerStats { get; private set; }
    public PlayerManager playerManager { get; private set; }
    private InputManager inputManager;


    private void Awake()
    {
        playerManager = FindFirstObjectByType<PlayerManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();
        inputManager = playerManager.GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        if (inputManager.invAction.IsPressed())
        {
            hudHandler.gameObject.SetActive(!hudHandler.enabled);
            inventoryManager.gameObject.SetActive(!inventoryManager.enabled);
        }

        if (playerStats.isJumpScared)
        {
            jumpScareUI.SetActive(true);
            hudHandler.gameObject.SetActive(false);
            inventoryManager.gameObject.SetActive(false);
        }
    }

}
