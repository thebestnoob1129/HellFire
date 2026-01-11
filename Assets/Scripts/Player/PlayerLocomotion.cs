using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;
    CapsuleCollider capsuleCollider;
    PlayerStats  playerStats;
    public Rigidbody playerRigidbody;

    private Transform cameraObject;
    private Transform cineObject;


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

    [Header("Jump Speeds")]
    public float jumpHeight = 3f;
    public float gravityIntensity = -15f;

    private float defaultHeight;
    private Vector3 defaultCenter;

    [Header("Interaction Flags")]
    public Transform targetObject;
    private Transform hideObject;
    public float interactRange = 1.5f;

    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerStats = GetComponent<PlayerStats>();

        cameraObject = Camera.main.transform;
        cineObject = playerManager.cineCamera.transform;

        rayCastHeightOffset =  capsuleCollider.height / 2;
        defaultHeight = capsuleCollider.height;
        defaultCenter = capsuleCollider.center;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        HandleFallingAndLanding();
        HandleInteract();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        if (isHiding)
        {
            if (inputManager.moveAmount > 0)
            {
                transform.position = hideObject.position + hideObject.forward;
                targetObject.SetParent(transform);
                targetObject.localPosition = new(0, 1.7f, 0);
                StartCoroutine(LeaveSpot());
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
        Ray hitRay = new()
        {
            origin = transform.position + new Vector3(0, rayCastHeightOffset, 0),
            direction = -transform.up
        };
        Vector3 targetPosition = transform.position + new Vector3(0, rayCastHeightOffset, 0);

        if (!isGrounded)
        {
            animatorManager.PlayTargetAnimation("Falling", true);
            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer += Time.deltaTime;
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
        if (isGrounded)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
        }
    }

    public void HandleCrouch()
    {
        if (!isGrounded) return;
        animatorManager.animator.SetBool("isCrouching", isCrouching);

        if (!isCrouching)
        {
            capsuleCollider.height = defaultHeight;
            capsuleCollider.center = defaultCenter;
            return;
        }

        //animatorManager.PlayTargetAnimation("Crouch Idle", false);
        capsuleCollider.height = defaultHeight / 2;
        capsuleCollider.center = new Vector3(defaultCenter.x, defaultCenter.y / 2, defaultCenter.z);

    }
    
    void HandleInteract()
    {
        if (isInteracting)
        {
            Ray r = new(cameraObject.position, cameraObject.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                GameObject obj = hitInfo.collider.gameObject;
                animatorManager.animator.SetBool("isInteracting", true);

                if (obj.TryGetComponent(out HidingSpot hidingSpot))
                {
                    Debug.Log("Hide", obj);
                    hideObject = hidingSpot.transform;
                    transform.position = hideObject.position + Vector3.up * 3;
                    targetObject.SetParent(hideObject, false);
                    targetObject.position = hideObject.position;
                    StartCoroutine(HidePlayer());
                }

                if (obj.TryGetComponent(out IInteractable interactObject))
                {
                    Debug.Log("Interacted", obj );
                    interactObject.Interact();
                }
                
            }
        }
        else
        {
            animatorManager.animator.SetBool("isInteracting", false);
        }
    }
    
    private readonly WaitForSeconds waitTime = new(1f);

    private IEnumerator HidePlayer()
    {
        animatorManager.animator.SetBool("isHiding", true);
        yield return waitTime;
        isHiding = true;
    }
    private IEnumerator LeaveSpot()
    {
        animatorManager.animator.SetBool("isHiding", false);
        yield return waitTime;
        isHiding = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (cameraObject)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(new Ray(cameraObject.position, cameraObject.forward * 3f));
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(cameraObject.position, interactRange);
        }
    }
}