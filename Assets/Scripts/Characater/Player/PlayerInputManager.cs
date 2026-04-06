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
        public bool sprintInput;
        public bool jumpInput;
        public bool attackInput;
        public bool crouchInput;
        public bool interactInput;
        public bool dropInput;
        public bool reloadInput;
        public bool switchRightWeapon;
        public bool switchLeftWeapon;


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
                
                playerControls.Player.Reload.performed += ctx => reloadInput = true;

                playerControls.Player.Jump.performed += ctx => jumpInput = true;

                // Actions
                playerControls.Player.SwitchRightWeapon.performed += ctx => switchRightWeapon = true;
                playerControls.Player.SwitchLeftWeapon.performed += ctx => switchLeftWeapon = true;


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
            HandleDropInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleReloadInput();
            HandleAttackInput();
            HandleSwitchRightWeaponInput();
            HandleSwitchLeftWeaponInput();
            HandleCrouchInput();

            // Attack
            player.isFiring = attackInput;
            // Interact
            player.isInteracting = interactInput;
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

        private void HandleCrouchInput()
        {
            if (player.isPerformingAction) return;
            if (!crouchInput) return;

            player.playerAnimatorManager.PlayTargetActionAnimation("Crouch", true, true);
        }

        // WARNING ADD ALL DRAIN STAMINA EFFECT TO EACH ATTACK ANIMATION EP.31

        private void HandleAttackInput()
        {
            if (isUIOpen) return;

            if (attackInput)
            {
                // ATTEMPT TO PERFORM ATTACK
                player.playerNetworkManager.SetCharacterActionHand(true);

                // Run Dual Wield Attack    
                if (player.playerNetworkManager.isUsingLeftHand.Value)
                {
                    player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentLeftWeaponItem.attackAction, player.playerInventoryManager.currentLeftWeaponItem);
                }
                
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeaponItem.attackAction, player.playerInventoryManager.currentRightWeaponItem);
                }


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

    }
}