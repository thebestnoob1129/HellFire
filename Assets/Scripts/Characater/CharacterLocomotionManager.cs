using System;
using UnityEngine;

namespace CFS
{
    public class CharacterLocomotionManager : MonoBehaviour
    {

        private CharacterManager character;

        [Header("Ground & Jumping")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] protected Vector3 yVelocity; // FORCE OF CUSTOM GRAVITY
        [SerializeField] private float groundCheckRadius = 0.1f;
        [SerializeField] protected float groundedYVelocity = -20f; // FORCE CHARACTER IS STICKING TO GROUND
        [SerializeField] protected float fallStartYVelocity = -5f; // FORCE TO BEGIN FALLING
        [SerializeField] protected float groundOffset = 0.1f;
        protected bool fallingVelocitySet = false;
        protected float inAirTimer = 0;


        protected virtual void Awake()
        {
            Debug.LogWarning("Ground Detection Based On Layer", gameObject);
            character = GetComponent<CharacterManager>();
            character.isGrounded = true;
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocitySet = false;
                    yVelocity.y = 0;
                }
            }
            else
            {
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocitySet)
                {
                    fallingVelocitySet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;
                character.animator.SetFloat(Animator.StringToHash("inAirTimer"), inAirTimer);
                yVelocity.y += Physics.gravity.y * Time.deltaTime;
            }
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {

            // Landing
            /*
            // Check if Ground is approaching
            if (Physics.SphereCast(transform.position, groundCheckRadius, -transform.up, out var hit, 3f))
            {

                if (character.isGrounded) return;

                // Slow Down Player To load Correctly
                yVelocity.y = Physics.gravity.y * Time.deltaTime;
            }
            */
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckRadius, WorldUtilityManager.Instance.GetEnviroLayers());
            //character.isGrounded = Physics.CheckSphere(character.transform.position - new Vector3(0, -2f, 0), groundCheckRadius, groundLayer);
        }

        // Reference :EP40
        public void EnableCanRotate()
        {
            character.canRotate = true;
        }

        public void DisableCanRotate()
        {
            character.canRotate = false;
        }

    }
}