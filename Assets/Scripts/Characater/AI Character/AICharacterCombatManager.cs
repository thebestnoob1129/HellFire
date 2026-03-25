using UnityEngine;

namespace CFS
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;

        [Header("Action Recovery")]
        public float actionRecoveryTimer = 0;
        public float actionRecoveryMultiplier = 0;

        [Header("Target Information")]
        public Vector3 targetsDirection;
        public float viewableAngle, distanceFromTarget;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 10f;
        public float minimumFOV = -35f;
        public float maximumFOV = 35f;

        [Header("Attack Rotation Speed")]
        [SerializeField] private float attackRotationSpeed = 10f;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null) return;

            var colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius,
                WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                var target = colliders[i].GetComponent<CharacterManager>();

                if (target == null) continue;
                if (target == aiCharacter) continue;
                if (target.isDead.Value) continue;

                if (WorldUtilityManager.Instance.CanIDamageTarget(aiCharacter.characterGroup, target.characterGroup))
                {
                    // If Target Is Found
                    var targetDirection = target.transform.position - aiCharacter.transform.position;
                    var viewAngle = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                    if (viewAngle >= minimumFOV && viewAngle <= maximumFOV)
                    {
                        // Check Environmental Blocks
                        if (Physics.Linecast(aiCharacter.characterCombatManager.head.position,
                                target.characterCombatManager.head.position, WorldUtilityManager.Instance.GetEnviroLayers()))
                        {
                            //Debug.Log("Blocked");
                        }
                        else
                        {
                            targetsDirection = currentTarget.transform.position - transform.position;
                            viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetsDirection);

                            aiCharacter.characterCombatManager.currentTarget = target;
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null) return;

            if (!aiCharacter.canRotate) return;

            if (!aiCharacter.isPerformingAction) return;

            // rotate towards target @ specified rotation speed during specified Frames
            var targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection == Vector3.zero) targetDirection = aiCharacter.transform.forward;

            var targetRotation = Quaternion.LookRotation(targetDirection);

            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation,
                attackRotationSpeed * Time.deltaTime);
        }

        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // Play Pivot Animation based on viewable angle
            if (aiCharacter.isPerformingAction) return;

            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
            }
            else if(viewableAngle <= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
            }
            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if(viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            else if (viewableAngle >= 110 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
            }
            else if(viewableAngle <= -110 && viewableAngle >= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
            }
            else if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if(viewableAngle <= -146 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }

        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}