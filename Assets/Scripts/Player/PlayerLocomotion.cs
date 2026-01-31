using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void Interact();
}

[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(AnimatorManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    private InputManager inputManager;// Handle Movement
    private AnimatorManager animatorManager;
    private CapsuleCollider capsuleCollider;
    private PlayerStats  playerStats;
    public Rigidbody playerRigidbody;

    private Transform cameraObject;
    private Transform cineObject;

    [Header("Inputs")] 
    [HideInInspector] private InputAction interactAction;

    [Header("Ground & Air Detection")] 
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Speeds")]
    private Vector3 lastPosition;
    public Vector3 moveDirection;
    public Vector3 playerVelocity;
    public float crouchSpeed = 0.5f, walkSpeed = 1.0f, runSpeed = 5f, sprintSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Movement Flags")]
    public bool isGrounded = true;
    public bool isSprinting, isCrouching, isInteracting, isJumping, isHiding;
    private Ray slopeRay;

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;
    
    [Header("Crouch")]
    private float defaultHeight;
    private Vector3 defaultColliderPos;
    private Vector3 defaultCameraPos;

    [Header("Interaction Flags")]
    [SerializeField] private Transform targetObject;
    private HidingSpot hideObject;
    public float interactRange = 1.5f;
    public LayerMask interactLayer;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerStats = GetComponent<PlayerStats>();

        // Cameras
        cameraObject = Camera.main.transform;
        cineObject = playerManager.cineCamera.transform;

        // Collider
        rayCastHeightOffset =  capsuleCollider.height / 2;
        defaultHeight = capsuleCollider.height;
        defaultColliderPos = capsuleCollider.center;
        defaultCameraPos = targetObject.localPosition;

        interactAction = inputManager.interactAction;
    }
    private void FixedUpdate()
    {
        HandleInteract();
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        HandleFallingAndLanding();
        HandleCrouch();
    }
    #region Movement
    private void HandleMovement()
    {
        if (isHiding)
        {
            if (inputManager.moveAmount > 0)
            {
                transform.position = hideObject.releasePositon + hideObject.transform.position;
                targetObject.SetParent(transform);
                targetObject.localPosition = new Vector3(0, 1.7f, 0);
                Invoke(nameof(LeaveSpot), 1f);
            }
            else
            {
                return;
            }
        }
        moveDirection = (cameraObject.forward * inputManager.verticalInput) + (cameraObject.right * inputManager.horizontalInput);
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting && playerStats.stamina > 0)
        { 
            playerVelocity = sprintSpeed * moveDirection;
            playerStats.stamina--;
        }
        else if (inputManager.moveAmount > 0.5f) { playerVelocity = runSpeed * moveDirection; }
        else if (isCrouching) { playerVelocity = crouchSpeed * moveDirection; }
        else{ playerVelocity = walkSpeed * moveDirection; }

        /*
        //Player Slope Rotation
        if (Physics.SphereCast(slopeRay, 0.2f, out var hitInfo, rayCastHeightOffset, groundLayer))
        {
            Vector3 slopeNormal = hitInfo.normal;
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, slopeNormal);
            playerVelocity = slopeRotation * playerVelocity;
            playerVelocity.y += rayCastHeightOffset;
        }
        */
        playerRigidbody.linearVelocity = playerVelocity;
    }
    
    private void HandleRotation()
    {
        Vector3 targetDirection = (cameraObject.forward * inputManager.verticalInput) + (cameraObject.right * inputManager.horizontalInput);
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
        slopeRay = new Ray{ origin = transform.position + new Vector3(0, rayCastHeightOffset, 0), direction = -transform.up };
        
        var targetPosition = transform.position;

        if (!isGrounded)
        {
            animatorManager.PlayTargetAnimation("Falling", true);
            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(Physics.gravity * (10 * inAirTimer), ForceMode.Impulse);
        }


        if (Physics.SphereCast(slopeRay, 0.2f, rayCastHeightOffset))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            targetPosition.y = hit.point.y + transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            //transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
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
            }
            else
            {
                transform.position = targetPosition;
            }
        }
        
    }

    public void HandleCrouch()
    {
        //if (!isGrounded) return;
        if (!isHiding) return;
        animatorManager.animator.SetBool("isCrouching", isCrouching);

        if (inputManager.crouchAction.inProgress)
        {
            Debug.Log("Crouching");
            capsuleCollider.height = defaultHeight / 2;
            capsuleCollider.center = new Vector3(defaultColliderPos.x, defaultColliderPos.y / 2, defaultColliderPos.z);
            targetObject.localPosition = new Vector3(0, defaultCameraPos.y / 2, 0);
            //animatorManager.PlayTargetAnimation("Crouch Idle", false);
        }
        else
        {
            capsuleCollider.height = defaultHeight;
            capsuleCollider.center = defaultColliderPos;
            targetObject.localPosition = defaultCameraPos;
        }

    }
    #endregion

    #region Interact
    private void HandleInteract()
    {
        if (interactAction.IsPressed())
        {
            Ray r = new(cameraObject.position, cameraObject.forward);
            if (Physics.Raycast(r, out var hitInfo, interactRange))
            {
                var obj = hitInfo.collider.gameObject;
                animatorManager.animator.SetBool("isInteracting", true);

                if (obj.TryGetComponent(out HidingSpot spot))
                {
                    StartCoroutine(HidePlayer(spot));
                    return;
                }

                if (obj.GetComponent<Item>())
                {
                    var canAdd = InventoryManager.Instance.AddItem(obj.GetComponent<Item>());
                    if (canAdd && canPickup)
                    {
                        Destroy(obj);
                        StartCoroutine(PickUpItem());
                        return;
                    }
                }

                if (obj.GetComponent<Pickup>()) { playerManager.Pickup(obj); }
            }
        }
        else
        {
            animatorManager.animator.SetBool("isInteracting", false);
        }
    }

    private bool canPickup = true;
    private readonly WaitForSeconds waitTime = new(1f);

    private IEnumerator PickUpItem()
    {
        if (!canPickup) yield break;
        canPickup = false;
        yield return waitTime;
        canPickup = true;
    }

    #endregion

    #region Hide
    private IEnumerator HidePlayer(HidingSpot hidingSpot)
    {
        hideObject = hidingSpot;
        transform.position = hidingSpot.transform.position + hidingSpot.releasePositon;
        targetObject.SetParent(hidingSpot.transform, false);
        targetObject.position = hidingSpot.transform.position;
        animatorManager.animator.SetBool("isHiding", true);
        yield return waitTime;
        isHiding = true;
    }
    private void LeaveSpot()
    {
        animatorManager.animator.SetBool("isHiding", false);
        isHiding = false;
    }

#endregion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(slopeRay);

        Gizmos.color = Color.yellow;
        if (cameraObject) Gizmos.DrawRay(new Ray(cameraObject.position, cameraObject.forward * interactRange));
    }
}