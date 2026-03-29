using UnityEngine;
using UnityEngine.AI;

namespace CFS
{
    [RequireComponent(typeof(AICharacterCombatManager))]
    [RequireComponent(typeof(AICharacterAnimationManager))]
    [RequireComponent(typeof(AICharacterNetworkManager))]
    [RequireComponent(typeof(AICharacterLocomotionManager))]
    public class AICharacterManager : CharacterManager
    {
        // Referencve EP: 40 Character Damaging
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
        [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
        [HideInInspector] public AICharacterAnimationManager aiCharacterAnimationManager;

        [Header("Navmesh Agent")]
        public NavMeshAgent navMeshAgent;

        [Header("Current State")]
        [SerializeField] private AIState currentState;

        [Header("States")]
        public IdleState idle;
        public PursueTargetState pursueTarget;
        public CombatStanceState combatStance;
        public AttackState attackStance;

        protected override void Awake()
        {
            base.Awake();

            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
            aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
            aiCharacterAnimationManager = GetComponent<AICharacterAnimationManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            animator.applyRootMotion = true;

            // Use a copy to not modify original object
            idle = Instantiate(idle);
            pursueTarget = Instantiate(pursueTarget);

            currentState = idle;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!IsOwner) return;

            ProcessStateMachine();
        }

        protected override void Update()
        {
            base.Update();

            aiCharacterCombatManager.HandleActionRecovery(this);
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value) return;

            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }


        private void ProcessStateMachine()
        {
            var nextState = currentState?.Tick(this);

            if (nextState != null)
            {
                currentState = nextState;
            }
            
            // Reset Position/Rotation
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;

            if (aiCharacterCombatManager.currentTarget != null)
            {
                aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                aiCharacterCombatManager.viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
                aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position,
                    aiCharacterCombatManager.currentTarget.transform.position);
            }

            if (navMeshAgent.enabled)
            {
                var agentDestination = navMeshAgent.destination;
                var remainingDistance = Vector3.Distance(agentDestination, transform.position);

                aiCharacterNetworkManager.isMoving.Value = remainingDistance > navMeshAgent.stoppingDistance;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }
        }
    }
}