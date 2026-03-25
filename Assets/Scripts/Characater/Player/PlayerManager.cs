using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace CFS
{
    [RequireComponent(typeof(PlayerLocomotionManager))]
    [RequireComponent(typeof(PlayerStatsManager))]
    [RequireComponent(typeof(PlayerAnimatorManager))]
    [RequireComponent(typeof(PlayerNetworkManager))]
    [RequireComponent(typeof(PlayerEffectsManager))]
    [RequireComponent(typeof(PlayerInventoryManager))]
    [RequireComponent(typeof(PlayerEquipmentManager))]
    [RequireComponent(typeof(PlayerSoundFXManager))]
    [RequireComponent(typeof(PlayerCombatManager))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class PlayerManager : CharacterManager
    {

        private PlayerInputManager inputManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        [HideInInspector] public PlayerSoundFXManager playerSoundFXManager;
        public CharacterController playerController { get; private set; }

        [Header("States")]
        public bool isHiding;

        [Header("Actions")]
        public bool isFiring;
        public bool isReloading;

        [Header("Items")]
        public Item[] itemList;
        public Item currentItem;

        [Header("Camera")]
        public bool canInteract;
        public bool isInteracting; 
        public bool isUsingRootMotion;

        [Header("Pickup")]
        public GameObject holdObject;
        public Transform holdPos;

        public float radiusSize;

        #region Update
        protected override void Awake()
        {
            base.Awake();
            Debug.LogWarning("Vitality Currently Doesn't Change"); // Character creation or level up system is not implemented yet, so vitality and endurance are not changing, which means health and stamina are not changing as well. This will be implemented in the future when the character progression system is added.
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerController = GetComponent<CharacterController>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerSoundFXManager = GetComponent<PlayerSoundFXManager>();

            inputManager = PlayerInputManager.Instance;
            animator = GetComponent<Animator>();
            SceneManager.activeSceneChanged += OnSceneChanged;
            DontDestroyOnLoad(gameObject);
        }
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            Debug.Log("Scene Changed: " + newScene.name);
            
            if (GameObject.FindGameObjectWithTag("Respawn")) transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position + new Vector3(0, 3, 0);
            
            Cursor.lockState = CursorLockMode.Confined;
        }

        protected override void Update()
        {
            base.Update();
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isInteracting", isInteracting);
            if (!IsOwner) return;
            
            inputManager.HandleAllInputs();
            isGrounded = playerController.isGrounded;

        }

        protected override void LateUpdate()
        {
            if (!IsOwner) return;
            base.LateUpdate();
            
            PlayerCamera.Instance.HandleAllCameraActions();

            animator.SetBool("isInteracting", isInteracting);
            isUsingRootMotion = animator.GetBool("isUsingRootMotion");
            playerNetworkManager.isJumping.Value = animator.GetBool("isJumping");
        }

        private void OnClientConnectedCallBack(ulong clientID)
        {
            // Keep a list of active players
            WorldGameSessionManager.Instance.AddPlayerToActivePlayerList(this);

            // Need To Use callback for players joining sever
            if (!IsServer && IsOwner)
            {
                foreach (var player in WorldGameSessionManager.Instance.players)
                {
                    if (player == this) return;

                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;

            // Flags
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockOnChanged;
            playerNetworkManager.currentLockOnTargetID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChanged;

            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnChargingAttackChanged;

            if (IsOwner)
            {
                PlayerCamera.Instance.player = this;
                PlayerInputManager.Instance.player = this;
                WorldSaveGameManager.Instance.player = this;

                gameObject.isStatic = true;

                name = WorldSaveGameManager.Instance.currentCharacterData.characterName;
                
                // Update the total amount of health or stamina when the stat linked is changed
                
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.Instance.playerHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.playerHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            // Status
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHealth;

            // Equipment
            playerNetworkManager.currentRightWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;


            // Upon connecting load data to owner of character ( joining another user)
            if (IsOwner && !IsServer)
            {
                LoadGameDataToCurrentCharacterData(ref WorldSaveGameManager.Instance.currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallBack;

            // Flags
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockOnChanged;
            playerNetworkManager.currentLockOnTargetID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChanged;

            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnChargingAttackChanged;

            if (IsOwner)
            {
                playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.Instance.playerHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.Instance.playerHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            }

            // Status
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHealth;

            // Equipment
            playerNetworkManager.currentRightWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;


        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.Instance.playerPopUpManager.SendYouDiedPopUp();
            }

            // Check for Players that are alive, Respawn

            return base.ProcessDeathEvent(manuallySelectDeathAnimation);

        }

        #endregion
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentSaveData)
        {
            currentSaveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentSaveData.characterName = playerNetworkManager.characterName.Value.ToString();

            currentSaveData.vitality = playerNetworkManager.vitality.Value;
            currentSaveData.endurance = playerNetworkManager.endurance.Value;
            currentSaveData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentSaveData.currentStamina = playerNetworkManager.currentStamina.Value;
        }

        public void LoadGameDataToCurrentCharacterData(ref CharacterSaveData currentSaveData)
        {
            playerNetworkManager.characterName.Value = currentSaveData.characterName ?? "Character";

            playerNetworkManager.vitality.Value = currentSaveData.vitality;
            playerNetworkManager.endurance.Value = currentSaveData.endurance;

            // MOVED ON SAVE DATA
            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(currentSaveData.vitality);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(currentSaveData.endurance);
            
            playerNetworkManager.currentStamina.Value = currentSaveData.currentStamina;
            playerNetworkManager.currentHealth.Value = currentSaveData.currentHealth;

            PlayerUIManager.Instance.playerHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }

        private void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            // Sync Weapons
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftWeaponID.Value);

            // Armor

            // LockON
            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChanged(0, playerNetworkManager.currentLockOnTargetID.Value);
            }
        }

        public override void ReviveCharacter()
        {
            if (!IsOwner) return;


            isDead.Value = false;
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            // Reset Flags

            // Reset Focus Points

            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }

    }
}