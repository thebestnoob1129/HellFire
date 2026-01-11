using UnityEngine;

[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(PlayerLocomotion))]
public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;

    public float verticalInput, horizontalInput;
    public float moveAmount;

    private Vector2 movementInput;
    public bool sprintInput, dodgeInput, crouchInput, interactInput;
    void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            //PlayerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            //PlayerControls.Player.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

            // Sprint Input
            playerControls.Player.Sprint.performed += ctx => sprintInput = true;
            playerControls.Player.Sprint.canceled += ctx => sprintInput = false;

            playerControls.Player.Crouch.performed += ctx => crouchInput = true;
            playerControls.Player.Crouch.canceled += ctx => crouchInput = false;

            playerControls.Player.Interact.performed += ctx => interactInput = true;
            playerControls.Player.Interact.canceled += ctx => interactInput = false;

        }
        playerControls.Enable();
    }
    void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();

        HandleMovementInput();
        HandleSprintInput();
        HandleCrouchInput();
        HandleInteractInput();

    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, sprintInput);
    }

    private void HandleSprintInput()  
    {
        playerLocomotion.isSprinting = sprintInput && moveAmount > 0.5f;
    }

    private void HandleInteractInput()
    {
        playerLocomotion.isInteracting = interactInput;
    }

    private void HandleCrouchInput()
    {
        playerLocomotion.isCrouching = crouchInput;
    }
}
