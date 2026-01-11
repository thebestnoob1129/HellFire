using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HUDHandler hudHandler;
    PlayerControls playerControls;
    public PlayerStats playerStats { get; private set; }
    public PlayerManager playerManager { get; private set; }

    void Awake()
    {
        playerControls = new PlayerControls();
        playerManager = FindFirstObjectByType<PlayerManager>();
        playerStats = playerManager.GetComponent<PlayerStats>();
    }
}
