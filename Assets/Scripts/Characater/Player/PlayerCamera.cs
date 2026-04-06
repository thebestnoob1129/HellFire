using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace CFS
{
	public class PlayerCamera : MonoBehaviour
	{
		public static PlayerCamera Instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] private Transform cameraPivotTransform;

        [Header("Camera Settings")]
        [SerializeField] private float cameraSmoothSpeed = 1f;
        [SerializeField] private float leftRightLookAngle = 220;
        [SerializeField] private float upDownLookAngle = 220;
        [SerializeField] private float minimumPivot = -30;
        [SerializeField] private float maximumPivot = 60;
        [SerializeField] private float cameraCollisionRadius = 3f;
        //[SerializeField] private bool invertCamera = false;
        [SerializeField] private LayerMask cameraLayer;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // CAMERA COLLISION
        [SerializeField] private float leftRightRotationSpeed = 10f;
        [SerializeField] private float upDownRotationSpeed = 10f;
        private float cameraZPosition; // CAMERA COLLISION
        private float targetZPosition; // CAMERA COLLISION

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
            DontDestroyOnLoad(transform.root.gameObject);
            cameraZPosition = cameraObject.transform.position.z;
        }

        public void HandleAllCameraActions()
        {
            if (!player) return;

            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }

        private void HandleFollowTarget()
        {
            var targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
                ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);

            transform.position = targetCameraPosition;
        }

        private void HandleRotation()
        {
            /*
            // IF LOCKED ON FORCE ROTATION TO TARGET
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // Rotates GameObject
                var rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.head.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0; // Left and Right

                var targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetSnapSpeed);

                // Rotates Pivot Object
                rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.head.position - cameraPivotTransform.transform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.rotation =
                    Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

                // Save Rotation Values to it Doesn't snap
                leftRightLookAngle = transform.eulerAngles.y;
                upDownLookAngle = transform.eulerAngles.x;

            }
            else
            */
            {
                //if (invertCamera) upDownRotationSpeed *= 1; else upDownRotationSpeed *= -1;

                leftRightLookAngle += (PlayerInputManager.Instance.cameraHorizontalInput * leftRightRotationSpeed) * Time.deltaTime;
                upDownLookAngle -= (PlayerInputManager.Instance.cameraVerticalInput * upDownRotationSpeed) * Time.deltaTime;

                upDownLookAngle = Mathf.Clamp(upDownLookAngle, minimumPivot, maximumPivot);

                var cameraRotation = Vector3.zero;

                // Vertical Rotation
                cameraRotation.y = leftRightLookAngle;
                var targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                // Horizontal Rotation
                cameraRotation = Vector3.zero;
                cameraRotation.x = upDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            
        }

        private void HandleCollisions()
        {
            targetZPosition = cameraZPosition;
            // DIRECTION OF CHECK
            var direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // CHECK IF OBJECT COLLIDES WITH CAMERA
            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out var hit,
                    Mathf.Abs(targetZPosition), cameraLayer))
            {
                // MOVE CAMERA DISTANCE
                var distanceFromObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetZPosition = -(distanceFromObject - cameraCollisionRadius);

            }

            if (Mathf.Abs(targetZPosition) > cameraCollisionRadius)
            {
                targetZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;

        }

        /*
        [Header("Lock On")]
        [SerializeField] private float lockOnTargetSnapSpeed = 0.3f;
        [SerializeField] private float lockOnTargetFollowSpeed = 0.3f;
        [SerializeField] private float lockOnRadius = 20;
        [SerializeField] private float minimumViewableAngle = -50;
        [SerializeField] private float maximumViewableAngle = 50;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public void HandleLocatingLockOnTargets()
        {
            var shortestDistance = Mathf.Infinity; // Used to determine the closest target
            var shortDistanceOfRightTarget = Mathf.Infinity; // Used to determine the shortest distance on one axis to the right (+)
            var shortDistanceOfLeftTarget = -Mathf.Infinity; // Used to determine the shortest distance on one axis to the left (-)

            // Use a Player Layer

            var colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius);//, WorldUtilityManager.Instance.GetCharacterLayers());
            
            // Add Available Target To List
            for (int i = 0; i < colliders.Length; i++)
            {
                var lockOnTarget = colliders[i].GetComponent<CharacterManager>();
                Debug.Log("Found Target: " + lockOnTarget, colliders[i].gameObject);

                // Check for field of view
                var lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                var distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                var viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                //if (lockOnTarget.isDead.Value) continue; // Check If Dead
                if (lockOnTarget.transform.root == player.transform.root) continue; // Lock On To Self
                //if (distanceFromTarget > lockOnRadius) continue; // Too Far From Player

                // If target is outside of field of view or blocked by environment 
                if (viewableAngle >= minimumViewableAngle && viewableAngle <= maximumViewableAngle)
                {
                    if (Physics.Linecast(player.playerCombatManager.head.position,
                            lockOnTarget.characterCombatManager.head.position, out var hit, WorldUtilityManager.Instance.GetEnviroLayers()))
                    {
                        // Can't View Target
                        continue;
                    }
                    else
                    {
                        // Add target to potential list
                        Debug.Log("Add Target");
                        availableTargets.Add(lockOnTarget);
                    }
                }
                


            }

            // Sort Through Potential Targets
            for (int i = 0; i < availableTargets.Count; i++)
            {

                if (availableTargets[i] == null) continue;

                var distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[i].transform.position);
                var lockTargetsDirection = availableTargets[i].transform.position - player.transform.position;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i];
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;
                }

            }

        }

        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            availableTargets.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, lockOnRadius);
        }

        */
    }
}