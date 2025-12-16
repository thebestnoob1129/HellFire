using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerStats playerStats;
    private PlayerControls playerControls;

    public TMP_Text healthText;
    public TMP_Text staminaText;

    public RectTransform healthBar;
    public RectTransform staminaBar;

    void Awake()
    {
        playerControls = new();
        playerManager = FindFirstObjectByType<PlayerManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();

    }

    void OnEnable()
    {
        playerControls.UI.Pause.performed += ctx => UIManager.Instance.TogglePauseMenu();
        playerControls.UI.Inventory.performed += ctx => UIManager.Instance.ToggleInventoryMenu();

        playerControls.Enable();
    }

    void Update()
    {
        HandleHealthUI();
        HandleStaminaUI();
    }

    void HandleHealthUI()
    {
        healthBar.anchorMax = new Vector2((float)playerStats.currentHealth / playerStats.MaxHealth, 1);
        healthText.text = playerStats.currentHealth.ToString() + " / " + playerStats.MaxHealth.ToString();
        healthBar.GetComponent<Image>().color = Color.Lerp(Color.darkRed, Color.forestGreen, (float)playerStats.currentHealth / playerStats.MaxHealth);
    }

    void HandleStaminaUI()
    {
        staminaBar.anchorMax = new Vector2((float)playerStats.currentStamina / playerStats.MaxStamina, 1);
        staminaText.text = playerStats.currentStamina.ToString() + " / " + playerStats.MaxStamina.ToString();
        staminaBar.GetComponent<Image>().color = Color.Lerp(Color.gray2, Color.lightSkyBlue, (float)playerStats.currentStamina / playerStats.MaxStamina);
    }

    void TogglePauseMenu()
    {

    }

    void ToggleInventoryMenu()
    {

    }

}
