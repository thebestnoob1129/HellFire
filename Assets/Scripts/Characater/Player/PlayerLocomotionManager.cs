using System.Collections;
using UnityEngine;

internal interface IInteractable
{
    public void Interact();
}

namespace CFS
{
    public class PlayerLocomotionManager: CharacterLocomotionManager
    {
        private PlayerManager player;
        private PlayerInputManager inputManager; // Handle Movement
        private CapsuleCollider capsuleCollider;
        private CharacterController controller;


        [Header("Movement Settings")]
        public Vector3 playerVelocity; // Movement
        private Vector3 lastPosition; // Hiding Spot
        public float crouchSpeed = 0.5f, walkSpeed = 1.0f, runSpeed = 5f, sprintSpeed = 7f;
        public float rotationSpeed = 15f, speedChangeRate = 5f;
        public float sprintStaminaCost = 1;
        [HideInInspector] public float verticalMovement, horizontalMovement, moveAmount;

        [Header("Jump Speeds")]
        [SerializeField] private float jumpStaminaCost = 10;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private float jumpForwardSpeed = 5;
        [SerializeField] private float fallForwardSpeed = 5;
        private Vector3 jumpDirection;


        [Header("Crouch")]
        private Vector3 defaultCenter; // Center is Half Height
        private Vector3 defaultCameraPos;

        [Header("Interaction Flags")]
        [SerializeField] private Transform targetObject;

        [Header("Dodge")]
        private Vector3 rollDirection;
        [SerializeField] private float dodgeStaminaCost = 25;

        private HidingSpot hideObject;
        public LayerMask interactLayer;
        public float interactRange = 1.5f;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            controller = GetComponent<CharacterController>();

            inputManager = PlayerInputManager.Instance;

            // Collider
            defaultCenter = controller.center;
            defaultCameraPos = targetObject.localPosition;
        }

        protected void Start()
        {
            lastPosition = transform.position;
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;

                // IF NOT LOCKED ON
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
                // IF LOCKED ON
                //player.playerAnimator.UpdateAnimatorMovementParamaters(horizontalMovement, verticalMovement);
            }

            GetMovementInput();
            HandleInteract();
            GroundMovement();
            HandleRotation();
            HandleJump();
            HandleFreeFallMovement();
        }

        #region Movement

        private void GetMovementInput()
        {
            verticalMovement = PlayerInputManager.Instance.verticalInput;
            horizontalMovement = PlayerInputManager.Instance.horizontalInput;
            moveAmount = PlayerInputManager.Instance.moveAmount;
            // Clamp Movement For Animations

        }

        private void GroundMovement()
        {
            if (!player.canMove) return;
            // Handle Hiding Spot
            if (player.isHiding)
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

            if (player.isCrouching)
            {

                // Controller Adjustments
                controller.center = new Vector3(0, defaultCenter.y / 2, 0);
                controller.height = defaultCenter.y;

                // Capsule adjustments
                capsuleCollider.center = new Vector3(0, defaultCenter.y / 2, 0);
                capsuleCollider.height = defaultCenter.y;

                // Camera
                targetObject.localPosition = new Vector3(0, defaultCameraPos.y / 2, 0);
                //animatorManager.PlayTargetAnimation("Crouch Idle", false);
            }
            else
            {
                // Controller Adjustments
                controller.center = defaultCenter;
                controller.height = defaultCenter.y * 2;
                // Capsule Adjustments
                capsuleCollider.center = defaultCenter;
                capsuleCollider.height = defaultCenter.y * 2;
                // Camera
                targetObject.localPosition = defaultCameraPos;
            }

            // Handle Movement
            var moveDirection = (PlayerCamera.Instance.cameraObject.transform.forward * inputManager.verticalInput) +
                                (PlayerCamera.Instance.cameraObject.transform.right * inputManager.horizontalInput);
           // moveDirection = cameraObject.transform.TransformDirection(moveDirection);
            moveDirection.Normalize();
            moveDirection.y = 0;

            // Speed Calculation
            var speed = 0f;
            /*
            if (player.isSprinting && player.playerStats. > 0)
            {
                speed = sprintSpeed;
                player.playerStats.stamina--;
            }
            else*/ if (inputManager.moveAmount > 0.5f)
            {
                speed = runSpeed;
            }
            else if (player.isCrouching)
            {
                speed = crouchSpeed;
            }
            else
            {
                speed = walkSpeed;
            }

            playerVelocity = speed * Time.deltaTime * moveDirection;
            controller.Move(playerVelocity);

        }

        private void HandleRotation()
        {
            if (!player.canRotate) return;

            var targetDirection = (PlayerCamera.Instance.cameraObject.transform.forward * inputManager.verticalInput) +
                                  (PlayerCamera.Instance.cameraObject.transform.right * inputManager.horizontalInput);
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }

            var targetRotation = Quaternion.LookRotation(targetDirection);
            var playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            player.playerNetworkManager.isSprinting.Value = moveAmount >= 0.1f;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintStaminaCost * Time.deltaTime;
            }

        }

        private void HandleJump()
        {
            if (player.playerNetworkManager.isJumping.Value)
            {
                player.characterController.Move(jumpForwardSpeed * Time.deltaTime * jumpDirection);
            }
        }

        private void HandleFreeFallMovement()
        {
            if (!player.isGrounded)
            {
                var freeFallDirection = PlayerCamera.Instance.transform.forward * PlayerInputManager.Instance.verticalInput;
                freeFallDirection += PlayerCamera.Instance.transform.right * PlayerInputManager.Instance.horizontalInput;
                freeFallDirection.y = 0;

                player.characterController.Move(fallForwardSpeed * Time.deltaTime * freeFallDirection);

            }
        }

        public void AttemptToPerformDodge()
        {
            if (player.isPerformingAction) return;
            if (player.playerNetworkManager.currentStamina.Value < dodgeStaminaCost) return;
            if (moveAmount > 0)
            {
                // ROLL
                rollDirection = PlayerCamera.Instance.cameraObject.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.Instance.cameraObject.transform.right * horizontalMovement;
                rollDirection.y = 0;
                rollDirection.Normalize();

                var playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward", true);

            }
            else
            {
                // PERFORM BACKSTEP ANIMATION
                player.playerAnimatorManager.PlayTargetActionAnimation("BackStep", true);

            }

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;

        }
        
        public void AttemptToPerformJump()
        {
            // CAN'T ATTACK WHILE JUMPING

            if (player.isPerformingAction) return;
            if (player.playerNetworkManager.currentStamina.Value < jumpStaminaCost) return;
            if (player.playerNetworkManager.isJumping.Value) return;
            if (!player.isGrounded) return;

            // IF 2 ITEMS, JUMP 2 ITEMS INSTEAD OF 1 ITEM
            
            player.playerAnimatorManager.PlayTargetActionAnimation("Main_Jump_01", false);

            player.playerNetworkManager.isJumping.Value = true;

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;

            jumpDirection = PlayerCamera.Instance.cameraObject.transform.forward * PlayerInputManager.Instance.verticalInput;
            jumpDirection += PlayerCamera.Instance.cameraObject.transform.right * PlayerInputManager.Instance.horizontalInput;

            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                // JUMP DISTANCE BASED ON MOVE SPEED
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                }
                else if (PlayerInputManager.Instance.moveAmount > 0.5f)
                {
                    jumpDirection *= 0.5f;
                }
                else
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
        }

        #endregion

        #region Interact

        private void HandleInteract()
        {
            var heightOffset = controller.center * 1.8f;
            Ray r = new(transform.position + heightOffset, transform.forward);
            player.canInteract = Physics.Raycast(r, out var hitInfo, interactRange);
            if (player.isInteracting)
            {
                if (!player.canInteract) return;
                
                var obj = hitInfo.collider.gameObject;

                if (obj.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Interact();
                }

                if (obj.TryGetComponent(out HidingSpot spot))
                {
                    StartCoroutine(HidePlayer(spot));
                    return;
                }
                /*
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

                if (obj.GetComponent<Pickup>())
                {
                    player.Pickup(obj);
                }
                */
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
            //animatorManager.animator.SetBool("isHiding", true);
            yield return waitTime;
            player.isHiding = true;
        }

        private void LeaveSpot()
        {
            //animatorManager.animator.SetBool("isHiding", false);
            player.isHiding = false;
        }

        #endregion
    }
}