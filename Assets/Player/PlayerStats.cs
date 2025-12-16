using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Header("Stats")]
    
    [SerializeField] private int maxHealth = 100; 
    public int MaxHealth { get { return maxHealth; } }
    public int currentHealth;
    public float healthRegen = 0.05f;

    [SerializeField] private int maxStamina = 100;
    public int MaxStamina { get { return maxStamina; } }
    public int currentStamina;
    public float staminaRegen = 0.05f;

    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandlePhysicalStats();
    }

    void HandlePhysicalStats()
    {
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // Logic to handle health and stamina regeneration or depletion can be added here

        // if (currentHealth < maxHealth) { StartCoroutine(RegenerateHealth()); }
        if (currentStamina < maxStamina) { StartCoroutine(RegenerateStamina()); }


    }


    private readonly WaitForSeconds regenTimeSeconds = new(5f);
    private readonly WaitForSeconds staminaTimeSeconds = new(2f);
    private readonly WaitForSeconds oneSecond = new(1f);
    IEnumerator RegenerateHealth()
    {
        yield return regenTimeSeconds; // Wait for 5 seconds before starting regeneration
        while (currentHealth < maxHealth)
        {
            currentHealth += Mathf.CeilToInt(healthRegen);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            yield return oneSecond; // Regenerate health every second
        }
    }
    IEnumerator RegenerateStamina()
    {
        yield return staminaTimeSeconds; // Wait for 2 seconds before starting regeneration
        while (currentStamina < maxStamina)
        {
            currentStamina += Mathf.CeilToInt(staminaRegen);
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            yield return oneSecond; // Regenerate stamina every second
        }
    }

}
