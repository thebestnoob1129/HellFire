using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;

    #region Movement

    [SerializeField, Min(0.1f)] private float walkSpeed;
    [SerializeField, Min(0.1f)] private float runSpeed;
    [SerializeField, Min(0.1f)] private float crouchSpeed;
    [SerializeField, Min(0.1f)] private float lookSpeed;
    [SerializeField, Min(1f)] private float jumpPower; 
    [SerializeField, Min(1f)] private float crouchHeight;
    [SerializeField, Min(1f)] private float defaultHeight;

    private Vector3 moveDirection;
    private bool canMove = true, canJump = true;

    [SerializeField] private float lookXLimit, rotationX;

    private PlayerInput playerInput;
    private InputAction moveAction, lookAction, sprintAction, crouchAction, jumpAction;
    

    public float activeGravity;

    #endregion

    #region Interaction

    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private GameObject interactObject;
    
    private RaycastHit hit;
    private InputAction interactAction;
    #endregion

    void Start()
    {
        //cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        sprintAction = playerInput.actions.FindAction("Sprint");
        crouchAction = playerInput.actions.FindAction("Crouch");
        jumpAction = playerInput.actions.FindAction("Jump");
        interactAction = playerInput.actions.FindAction("Interact");

        defaultHeight = characterController.height;
        crouchHeight = defaultHeight / 2;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        #region Movement
        Vector2 move = moveAction.ReadValue<Vector2>();
        Vector2 look = lookAction.ReadValue<Vector2>();

        transform.forward = Vector3.ProjectOnPlane(new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z), Vector3.up).normalized;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        bool isRunning = sprintAction.IsPressed() && !crouchAction.IsPressed();
        bool isCrouching = crouchAction.IsPressed() && !sprintAction.IsPressed();
        bool isJumping = jumpAction.IsPressed();

        // Physics Movement

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * move.y : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * move.x : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        float moveY = moveDirection.y;


        // Jump Action
        if (isJumping && canMove && canJump && characterController.isGrounded)
        {

            moveDirection.y = jumpPower;

            // Play Jump Animation
            //animator.Play("Armature|Jump");
        }
        else
        {
            moveDirection.y = moveY;
        }


        // Grounded
        if (!characterController.isGrounded)
        {
            activeGravity = Physics.gravity.y * (Time.deltaTime / 0.1f);
            moveDirection.y += activeGravity;
            // Play Airtime Animation
            //animator.Play("Armature|Air");
        }

        // Crouching
        if (isCrouching && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

            // Toggle Crouch Animation
            //animator.Play("Armature|Crouch");
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
            // Disable Crouch Animation
        }

        // Moving
        /*
        if (canMove)
        {
            rotationX += -look.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            //_cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, look.x * lookSpeed, 0);
        }
        */

        // Animator
        if (moveAction.IsPressed() && !animator.GetBool("isWalking")) { animator.SetTrigger("Walk"); }
        if (isJumping) { animator.SetTrigger("Jump"); }


        animator.SetBool("isWalking", moveAction.IsPressed());
        animator.SetBool("isRunning", sprintAction.IsPressed());
        animator.SetBool("isCrouching", crouchAction.IsPressed());

        characterController.Move(moveDirection * Time.deltaTime);
        #endregion

        #region Interaction

        // NPC Interaction

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactionRange))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                // Debug.Log("Looking at Interactable: " + hit.collider.name);
                if (interactAction.IsPressed())
                {
                    //Interactable.OnInteract(interactAction.started);
                    
                    // Debug.Log("Interacted with: " + hit.collider.name);
                    if (hit.collider.TryGetComponent<Interactable>(out var interactable))
                    {
                        interactable.OnInteract(new InputAction.CallbackContext());
                        interactObject = hit.collider.gameObject;
                        //interactable.OnInteract();
                    }
                }
            }
        }

        // Object Interaction

        #endregion
    }

    

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, cam.transform.forward * interactionRange);

        Gizmos.color = Color.goldenRod;
        Gizmos.DrawSphere(transform.position * interactionRange, 0.1f);
    }
}
