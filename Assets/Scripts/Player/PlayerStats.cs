using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Header("Stats")]
    public readonly int maxHealth = 5;
    public int health { get; private set; }
    public float healthRegen = 0.05f;
    bool isRegeneratingHealth = false;

    public readonly int maxStamina = 100;
    public int stamina;
    public float staminaRegen = 0.05f;
    bool isRegeneratingStamina = false;

    void Awake()
    {
        health = maxHealth;
        stamina = maxStamina;
    }

    void Update()
    {
        HandlePhysicalStats();
        if (health >= maxHealth) { isRegeneratingHealth = false; }
        if (stamina >= maxStamina) { isRegeneratingStamina = false; }
    }

    void HandlePhysicalStats()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (health < maxHealth && isRegeneratingHealth == false) { StartCoroutine(RegenerateHealth()); }
        if (stamina < maxStamina && isRegeneratingStamina == false) { StartCoroutine(RegenerateStamina()); }
    }

    private readonly WaitForSeconds regenTime = new(5f);
    private readonly WaitForSeconds oneSecond = new(1f);


    IEnumerator RegenerateHealth()
    {
        isRegeneratingHealth = true;
        yield return regenTime;
        while (health < maxHealth)
        {
            health += Mathf.CeilToInt(healthRegen);
            health = Mathf.Clamp(health, 0, maxHealth);
            yield return oneSecond; // Regenerate health every second
        }
    }
    IEnumerator RegenerateStamina()
    {
        isRegeneratingStamina = true;
        yield return regenTime;
        while (stamina < maxStamina)
        {
            stamina += Mathf.CeilToInt(staminaRegen);
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            yield return oneSecond; // Regenerate stamina every second
        }
    }

}
