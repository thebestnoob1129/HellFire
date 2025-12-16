using UnityEngine;

[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(PlayerLocomotion))]
public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private PlayerLocomotion playerLocomotion;
    private AnimatorManager animatorManager;

    private Vector2 movementInput;
    public float verticalInput, horizontalInput;
    public float moveAmount;

    public bool sprintInput, jumpInput, dodgeInput, crouchInput;
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

            playerControls.Player.Jump.performed += ctx => jumpInput = true;

            playerControls.Player.Dodge.performed += ctx => dodgeInput = true;

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
        HandleJumpInput();
        HandleDodgeInput();
        HandleCrouchInput();

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

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            playerLocomotion.HandleDodge();
        }
    }

    private void HandleCrouchInput()
    {
        playerLocomotion.isCrouching = crouchInput;
    }

}
