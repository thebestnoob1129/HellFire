using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    [SerializeField] private UIManager manager;

    PlayerManager playerManager;
    PlayerStats playerStats;

    public GameObject[] healthBar = new GameObject[5];
    public GameObject staminaBar;
    public GameObject interactLabel;

    void Start()
    {
        
        playerStats = manager.playerStats;
    }

    private void FixedUpdate()
    {
        HandleHealth();
        HandleStamina();
        HandleInteract();
    }

    private void HandleHealth()
    {
        // if player damaged to hit animation from ui
        var health = playerStats.health;
        var maxHealth = playerStats.maxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            if (i <= health)
            {
                healthBar[i].SetActive(true);
            }
            else
            {
                healthBar[i].SetActive(false);
            }
            Color activeColor = Color.Lerp(Color.darkRed, Color.forestGreen, (float)health / maxHealth);
            healthBar[i].GetComponent<RawImage>().color = activeColor;
        }
    }

    private void HandleStamina()
    {
         var stamina = playerStats.stamina;
         var maxStamina = playerStats.maxStamina;

         staminaBar.GetComponent<RectTransform>().anchorMax = new Vector2( stamina / maxStamina, 1);
         staminaBar.GetComponent<RawImage>().color = Color.Lerp(Color.black, Color.cyan, stamina / maxStamina);
    }

    private void HandleInteract()
    {
        Ray cameraRay = new()
        {
            direction = Camera.main.transform.forward,
            origin = Camera.main.transform.position
        };

        if (Physics.Raycast(cameraRay, out RaycastHit hit, 1.5f))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj == null) return;
            if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactLabel.SetActive(true);
                if (obj.GetComponent<HidingSpot>())
                {
                    interactLabel.GetComponent<TMP_Text>().text = "Hide";
                }
                else
                {
                    interactLabel.GetComponent<TMP_Text>().text = "Interact";
                }
                // Include Pick up

            }
            else
            {
                interactLabel.SetActive(false);
            }
        }
    }

    private readonly WaitForSeconds damageTime = new(1f);
    IEnumerator OnDamage()
    {
        yield return damageTime;
    }

}
