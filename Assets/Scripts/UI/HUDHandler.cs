using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{
    private GameObject player;
    private PlayerStats playerStats;
    private InputManager inputManager;
    private Ray cameraRay;
    private float interactRange = 3f;

    [Header("Health Info")]
    public GameObject healthBar;
    public Color healthHighColor, healthLowColor;

    [Header("Stamina Info")]
    public GameObject staminaBar;
    public Color staminaHighColor, staminaLowColor;

    [Header("Interact")]
    public GameObject interactLabel;
    public LayerMask interactLayerMask, entityLayer;
    public GameObject crosshair;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (!player) Debug.LogError("Player Not Found", gameObject);
        playerStats = player.GetComponent<PlayerStats>();
        interactRange = player.GetComponent<PlayerLocomotion>().interactRange;
        inputManager = player.GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        
        HandleHealth();
        HandleStamina();
        HandleInteract();
        HandleCrosshair();
    }

    private void HandleHealth()
    {
        // if player damaged to hit animation from ui
        var value = playerStats.health / playerStats.maxHealth;

        healthBar.GetComponent<RectTransform>().anchorMax = new Vector2(value, 1);
        healthBar.GetComponent<RawImage>().color = Color.Lerp(healthLowColor, healthHighColor, value);
    }

    private void HandleStamina()
    {

         var value = playerStats.stamina / playerStats.maxStamina;

         staminaBar.GetComponent<RectTransform>().anchorMax = new Vector2( value, 1);
         staminaBar.GetComponent<RawImage>().color = Color.Lerp(staminaLowColor, staminaHighColor, value);
        
    }

    private void HandleInteract()
    {

        cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(cameraRay, out var hit, interactRange, interactLayerMask))
        {
            var obj = hit.collider.gameObject;
            interactLabel.SetActive(true);
            interactLabel.GetComponent<TMP_Text>().text = obj.name;


            if (inputManager.interactAction.IsPressed() && obj.TryGetComponent(out IInteractable interactObject))
            {
                interactObject.Interact();
            }

        }
        else
        {
            interactLabel.SetActive(false);
        }
    }

    private void HandleCrosshair()
    {
        var cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(cameraRay, out var hit, 100, entityLayer))
        {
            crosshair.GetComponent<Image>().color = hit.collider.GetComponent<Entity>() ? Color.darkRed : Color.white;
        }
    }

    private readonly WaitForSeconds damageTime = new(1f);
    private IEnumerator OnDamage()
    {
        yield return damageTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(cameraRay.origin, cameraRay.origin + cameraRay.direction * interactRange);
    }
}
