using UnityEngine;

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;
    CapsuleCollider capsuleCollider;
    PlayerStats  playerStats;
    public Rigidbody playerRigidbody;

    Transform cameraObject;

    [Header("Ground & Air Detection")] 
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;

    public LayerMask groundLayer;

    [Header("Movement Speeds")]
    public Vector3 moveDirection;
    public float crouchSpeed = 0.5f, walkSpeed = 1.0f, runSpeed = 5f, sprintSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded = true;
    public bool isJumping;
    public bool isCrouching;

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;

    private float defaultHeight;
    private Vector3 defaultCenter;

    void Awake()
    {
        Debug.LogWarning("Ground is Detected By Layer Instead Of Collider");
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerStats = GetComponent<PlayerStats>();
        cameraObject = Camera.main.transform;

        rayCastHeightOffset =  GetComponent<CapsuleCollider>().height / 2;
        defaultHeight = capsuleCollider.height;
        defaultCenter = capsuleCollider.center;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        HandleFallingAndLanding();
        HandleCrouch();
        HandleDodge();
    }
    private void HandleMovement()
    {
        moveDirection = (cameraObject.forward * inputManager.verticalInput) + (cameraObject.right * inputManager.horizontalInput);
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting && playerStats.currentStamina > 0)
        { 
            moveDirection *= sprintSpeed;
            playerStats.currentStamina--;
        }
        else if (inputManager.moveAmount > 0.5f) { moveDirection *= runSpeed; }
        else if (isCrouching) { moveDirection *= crouchSpeed; }
        else{ moveDirection *= walkSpeed; }

        playerRigidbody.linearVelocity = moveDirection;

    }

    private void HandleRotation()
    {
        if (isJumping) return;
        Vector3 targetDirection;
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit = new();
        Ray hitRay = new()
        {
            origin = transform.position + new Vector3(0, rayCastHeightOffset, 0),
            direction = -transform.up
        };

        Vector3 targetPosition = transform.position + new Vector3(0, rayCastHeightOffset, 0);

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer += Time.deltaTime;

            playerRigidbody.AddForce(transform.forward * leapingVelocity, ForceMode.Impulse);
            playerRigidbody.AddForce(Physics.gravity * inAirTimer, ForceMode.Impulse);

        }


        if (Physics.SphereCast(hitRay, 0.2f, 1f, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            Vector3 raycastHitPoint = hit.point;
            targetPosition.y = raycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
        }
    }

    public void HandleJumping()
    {
        if (!isGrounded) return;

        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump", true);

        float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
        //playerRigidbody.linearVelocity += new Vector3(0, jumpingVelocity, 0);
        playerRigidbody.AddForce(new Vector3(0, jumpingVelocity, 0),ForceMode.VelocityChange);

    }

    public void HandleCrouch()
    {
        if (playerManager.isInteracting) return;
        if (!isGrounded) return;

        if (isCrouching)
        {
            animatorManager.animator.SetBool("isCrouching", isCrouching);
            //animatorManager.PlayTargetAnimation("Crouch Idle", false);
            
            capsuleCollider.height = defaultHeight / 2;
            capsuleCollider.center = defaultCenter / 2;
        }

        capsuleCollider.height = defaultHeight;
        capsuleCollider.center = defaultCenter;

    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting) return;

        animatorManager.PlayTargetAnimation("Dodge", true, true);

        // Root Motion

    }

}
