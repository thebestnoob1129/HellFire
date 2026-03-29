using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    [RequireComponent(typeof(CharacterStatsManager))]
    [RequireComponent(typeof(CharacterNetworkManager))]
    [RequireComponent(typeof(CharacterEffectsManager))]
    [RequireComponent(typeof(CharacterAnimatorManager))]
    [RequireComponent(typeof(CharacterCombatManager))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterSoundFXManager))]
    [RequireComponent(typeof(CharacterLocomotionManager))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class CharacterManager : NetworkBehaviour
    {
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [HideInInspector] public Animator animator;

        public CharacterNetworkManager characterNetworkManager;

        [Header("Status")]
        public CharacterGroup characterGroup;
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        // Can Move To Correct Scripts
        [Header("Flag")]
        public bool isPerformingAction = false;
        public bool applyRootMotion = false;
        public bool canRotate = true;
        public bool canMove = true;

        [Header("State")]
        public bool isGrounded;
        public bool isSprinting;
        public bool isCrouching;

        protected virtual void Awake()
        {
            //DontDestroyOnLoad(this);
            characterController = GetComponent<CharacterController>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            animator = GetComponent<Animator>();

            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;

        }

        protected virtual void Start()
        {
            IgnoreMyColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isJumping", characterNetworkManager.isJumping.Value);
            // IF CHARACTER IS CONTROLLED BY OWNER SET POSITION ON HOST, ELSE GET POSITION FROM HOST AND SET CLIENT
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                // Positon
                transform.position = Vector3.SmoothDamp
                    (transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);
                // Rotation
                transform.rotation = Quaternion.Slerp
                    (transform.rotation, 
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkRotationSmoothTime);
            }

            characterStatsManager.RegenerateStamina();

        }

        protected virtual void FixedUpdate()
        {

        }
        protected virtual void LateUpdate()
        {

        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // Reset Any required Flags

                // If we are not grounded, play aerial death animation

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }

            }
            // Play Death SFX

            // Play Death VFX

            yield return new WaitForSeconds(5f);

            // Award or Finish any required objectives

            // Disable Character
        }

        public virtual void ReviveCharacter()
        {

        }

        protected virtual void IgnoreMyColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damagableCharacter = GetComponentsInChildren<Collider>();
            List<Collider> ignoreColliders = new List<Collider>();

            // Adds all damagable colliders to the list, ignores all colliders on character prefab
            foreach (var collider in damagableCharacter)
            {
                ignoreColliders.Add(collider);
            }
            
            ignoreColliders.Add(characterControllerCollider);

            // Goes through all colliders and ignore all other colliders in list
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
    }
}