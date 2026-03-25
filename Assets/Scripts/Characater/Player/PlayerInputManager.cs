using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace CFS
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;

        public PlayerManager player;
        private PlayerControls playerControls;

        private bool isUIOpen;

        [Header("Camera Movement Input")]
        [SerializeField] private Vector2 cameraInput;
        public float cameraVerticalInput, cameraHorizontalInput;

        [Header("Lock On Input")]
        [SerializeField] private bool lockOnInput;

        [Header("Player Movement Values")] 
        [SerializeField] private Vector2 movementInput;
        public float verticalInput, horizontalInput;
        public float moveAmount;

        [Header("Player Action Input")]
        public bool dodgeInput;
        public bool sprintInput;
        public bool jumpInput;
        public bool attackInput;
        public bool crouchInput;
        public bool interactInput;
        public bool dropInput;
        public bool reloadInput;
        public bool switchRightWeapon;
        public bool switchLeftWeapon;


        [Header("Bumper Input")]
        public bool rbInput;

        [Header(" Trigger Input")]
        public bool rtInput;
        public bool holdRtInput;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
            Instance.enabled = false;
            if (playerControls != null) { playerControls.Disable();}

        }
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            // IF LOADING INTO WORLD SCENE ENABLE PLAYER CONTROLS
            Instance.enabled = newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex();
            if (playerControls != null)
            {
                playerControls.Enable();
            }

        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                
                playerControls.Player.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                playerControls.Player.Dodge.performed += ctx => dodgeInput = true;
                
                playerControls.Player.Reload.performed += ctx => reloadInput = true;

                playerControls.Player.Jump.performed += ctx => jumpInput = true;

                // Actions
                playerControls.Player.SwitchRightWeapon.performed += ctx => switchRightWeapon = true;
                playerControls.Player.SwitchLeftWeapon.performed += ctx => switchLeftWeapon = true;

                // Bumpers
                playerControls.Player.RB.performed += ctx => rbInput = true;

                // Trigger
                playerControls.Player.RT.performed += ctx => rtInput = true;
                playerControls.Player.HoldRT.performed += ctx => holdRtInput = true;
                playerControls.Player.HoldRT.canceled += ctx => holdRtInput = false;

                // Lock ON
                playerControls.Player.LockOn.performed += ctx => lockOnInput = true;

                // HOLD INPUT ACTIONS
                playerControls.Player.Sprint.performed += ctx => sprintInput = true;
                playerControls.Player.Sprint.canceled += ctx => sprintInput = false;

                playerControls.Player.Attack.performed += ctx => attackInput = true;
                playerControls.Player.Attack.canceled += ctx => attackInput = false;

                playerControls.Player.Crouch.performed += ctx => crouchInput = true;
                playerControls.Player.Crouch.canceled += ctx => crouchInput = false;

                playerControls.Player.Interact.performed += ctx => interactInput = true;
                playerControls.Player.Interact.canceled += ctx => interactInput = false;

                playerControls.Player.Drop.performed += ctx => dropInput = true;


            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            if (playerControls == null) Debug.LogError("No PLayer Controls?", gameObject);
            playerControls.Disable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        public void HandleAllInputs()
        {

            HandleMovementInput();
            HandleCameraInput();
            HandleDodgeInput();
            HandleDropInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleReloadInput();
            HandleRBInput();
            HandleRTInput();
            HandleChargeRTInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();

            // Not Completely Implemented
            //HandleLockOnInput();

            // Attack
            player.isFiring = attackInput;
            // Interact
            player.isInteracting = interactInput;
            // Crouch
            player.isCrouching = crouchInput;
            // Drop
        }

        private void HandleLockOnInput()
        {
            // Check For Dead Target
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null) return;

                if (player.playerCombatManager.currentTarget.isDead.Value)
                {

                }

                // Attempt to Find New Target
            }
            

            if (lockOnInput && player.playerNetworkManager.isLockedOn.Value)
            {
                Debug.Log("Disable Lock On");
                lockOnInput = false;
                PlayerCamera.Instance.ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
                // Disable Lock On
                return;
            }
            
            if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
            {
                Debug.Log("Enable Lock On");
                lockOnInput = false;
                // Enable Lock On
                PlayerCamera.Instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.Instance.nearestLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.Instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;

                }
            }


        }
        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

            if (player == null) return;
            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
            }
            else
            {
                player.playerNetworkManager.isMoving.Value = false;
            }

            // Sprint
            player.isSprinting = sprintInput && moveAmount > 0.5f;
            
            // IF NOT LOCKED ON
            if (!player) return;
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            // IF LOCKED ON
            //player.playerAnimator.UpdateAnimatorMovementParameters(horizontalInput, verticalInput);
        }

        private void HandleCameraInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        private void HandleDropInput()
        {
            if (!dropInput) return;
            //player.HandleDrop();
            dropInput = false;
        }
        
        private void HandleDodgeInput()
        {
            if (dodgeInput)
            {
                dodgeInput = false;

                // FUTURE NOTE: RETURN IF MENU OR UI WINDOW IS OPEN, DO NOTHING
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void HandleReloadInput()
        {
            if (reloadInput)
            {
                reloadInput = false;
            }
        }

        private void HandleSprintInput()
        {
            if (sprintInput)
            {
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jumpInput)
            {
                jumpInput = false;

                // IF UI IS OPEN RETURN

                // ATTEMPT TO JUMP
                player.playerLocomotionManager.AttemptToPerformJump();

            }
        }

        // WARNING ADD ALL DRAIN STAMINA EFFECT TO EACH ATTACK ANIMATION EP.31
        
        private void HandleRBInput()
        {
            if (isUIOpen) return;

            if (rbInput)
            {
                rbInput = false;

                player.playerNetworkManager.SetCharacterActionHand(true);

                // Run Two Handing if Two Handed

                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeaponItem.ohRBAction, player.playerInventoryManager.currentRightWeaponItem);
            }
        }
        private void HandleRTInput()
        {
            if (isUIOpen) return;

            if (rtInput)
            {
                rtInput = false;

               
            }
        }
        private void HandleSwitchRightWeaponInput()
        {
            if (isUIOpen) return;

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                player.playerEquipmentManager.SwitchRightWeapon();
               
            }
        }
        private void HandleSwitchLeftWeaponInput()
        {
            if (isUIOpen) return;

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                player.playerEquipmentManager.SwitchLeftWeapon();
            }
        }

        private void HandleChargeRTInput()
        {
            // Only Checking for charged input
            if (player.isPerformingAction)
            {
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerNetworkManager.isChargingAttack.Value = true;
                }
            }
        }
    }
}