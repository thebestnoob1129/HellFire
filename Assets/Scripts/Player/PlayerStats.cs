using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    [Header("Info")]
    public int PlayerId { get; private set;}
    public string playerName = "Player";
    public int Level { get; private set; } = 1;
    public int Xp { get; private set; } = 0;
    public int Currency { get; private set; } = 0;

    [Header("Health")]
    public float health { get; private set; }
    public readonly float maxHealth = 100;
    public float healthRegen = 0.05f;
    private bool isRegeneratingHealth = false;

    [Header("Stamina")]
    public float stamina;
    public readonly float maxStamina = 100;
    public float staminaRegen = 0.05f;
    private bool isRegeneratingStamina = false;

    [Header("Effects")]
    public bool isJumpScared { get; private set; }

    private void Awake()
    {
        health = maxHealth;
        stamina = maxStamina;
        name = playerName;
        Debug.LogWarning("IEnumerators");
    }

    private void Update()
    {
        HandlePhysicalStats();
        
        HandleStats();

        //HandleStatusEffects();

    }
    #region Stats
    private void HandlePhysicalStats()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        if (health >= maxHealth) { isRegeneratingHealth = false; }
        if (stamina >= maxStamina) { isRegeneratingStamina = false; }

        if (health < maxHealth && !isRegeneratingHealth) { StartCoroutine(RegenerateHealth()); }
        if (stamina < maxStamina && !isRegeneratingStamina) { StartCoroutine(RegenerateStamina()); }

        if (health <= 0)
        {
            Die();
        }
    }

    private void HandleStats()
    {
        // Level
        Level = Mathf.Clamp(Level, 1, 255);
        // Xp
        Xp = Mathf.Clamp(Xp, 0, int.MaxValue);

        if (Xp >= Level * 100)
        {
            Xp -= Level * 100;
            Level++;
        }

        // Currency
        Currency = Mathf.Clamp(Currency, 0, int.MaxValue);
    }

    public void AddXp(int value)
    {
        Xp += value;
    }

    #endregion

    #region Phyisical
    public void OnDamaged(float value)
    {
        health -= value;
    }

    private void Die()
    {
        Debug.Log("Player is Dead", gameObject);
    }
    #endregion
    private void OnCollisionEnter(Collision other)
    {
        var obj = other.collider.gameObject;

        if (obj.TryGetComponent<Bullet>(out var bullet))
        {
            OnDamaged(bullet.damage);
        }

        if (obj.TryGetComponent<Entity>(out var entity))
        {
            isJumpScared = entity.canJumpScare;
        }
    }

    private readonly WaitForSeconds regenTime = new(5f);
    private readonly WaitForSeconds oneSecond = new(1f);
    private IEnumerator RegenerateHealth()
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
    private IEnumerator RegenerateStamina()
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