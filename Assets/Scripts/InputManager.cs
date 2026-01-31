using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(PlayerLocomotion))]
public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerLocomotion playerLocomotion;
    private PlayerManager playerManager;
    private AnimatorManager animatorManager;

    public float verticalInput, horizontalInput;
    public float moveAmount;

    [Header("Inputs")]
    private Vector2 movementInput;

    public bool
        attackInput,
        sprintInput,
        dodgeInput,
        crouchInput,
        interactInput,
        dropInput,
        reloadInput;

    [HideInInspector] public InputAction
        interactAction,
        invAction,
        nextAction,
        prevAction,
        crouchAction;

    
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerManager = GetComponent<PlayerManager>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            invAction = playerControls.UI.Inventory;
            nextAction = playerControls.Player.Next;
            prevAction = playerControls.Player.Previous;
            interactAction = playerControls.Player.Interact;
            crouchAction = playerControls.Player.Crouch;

            playerControls.Player.Attack.performed += ctx => attackInput = true;
            playerControls.Player.Attack.canceled += ctx => attackInput = false;
            
            playerControls.Player.Reload.performed += ctx => reloadInput = true;
            playerControls.Player.Reload.canceled += ctx => reloadInput = false;

            playerControls.Player.Sprint.performed += ctx => sprintInput = true;
            playerControls.Player.Sprint.canceled += ctx => sprintInput = false;

            crouchAction.performed += ctx => crouchInput = true;
            crouchAction.canceled += ctx => crouchInput = false;

            interactAction.performed += ctx => interactInput = true;
            interactAction.canceled += ctx => interactInput = false;

            playerControls.Player.Drop.performed += ctx => dropInput = true;


        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();

        HandleMovementInput();
        // Attack
        playerManager.isFiring = attackInput;
        // Reload
        playerManager.isReloading = reloadInput;
        // Sprint
        playerLocomotion.isSprinting = sprintInput && moveAmount > 0.5f;
        // Interact
        playerLocomotion.isInteracting = interactInput;
        // Crouch
        playerLocomotion.isCrouching = crouchInput;
        HandleCrouchInput();
        // Drop
        HandleDropInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, sprintInput);
    }
    private void HandleDropInput()
    {
        if (!dropInput) return;
        playerManager.HandleDrop();
        dropInput = false;
    }

    private void HandleCrouchInput()
    {
        if (!crouchInput) return;

        playerLocomotion.HandleCrouch();

    }

}
