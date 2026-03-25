using UnityEngine;
using UnityEngine.AI;

namespace CFS
{
    [CreateAssetMenu(menuName = "A.I/State/Pursue")]
    public class PursueTargetState : AIState
    {

        public bool canPivotTarget = true;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Check if we are performing action
            if (aiCharacter.isPerformingAction) return this;

            // Check if target is null, else return to idle
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null) return this;

            // make sure navmesh agent is active else enable id
            if (aiCharacter.navMeshAgent.enabled == false) aiCharacter.navMeshAgent.enabled = true;

            // Disable Infinite Circle Enemy | Pivot to target if outside of field of view
            if (canPivotTarget)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle <
                    aiCharacter.aiCharacterCombatManager.minimumFOV ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle >
                    aiCharacter.aiCharacterCombatManager.maximumFOV)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // if in combat range, switch to combat state
            /* 
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStance.maxEngageDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            */
            // Melee Enemies
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            // if target is not reachable and ai is far from home, return home

            // pursue target

            var path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            if (path.status == NavMeshPathStatus.PathComplete) aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        /*
        protected bool IsPathWalkable(Vector3 value)
        {
            var path = new NavMeshPath();
            navAgent.CalculatePath(value, path);

            return path.status == NavMeshPathStatus.PathComplete;
        }
        */
    }
}