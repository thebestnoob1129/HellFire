using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CFS
{
    [CreateAssetMenu(menuName = "A.I/State/Combat")]
    public class CombatStanceState : AIState
    {

        // Select an Attack for the attack state, based on distance and angle of target to character
        // Process combat logic here while waiting to attack (blocking, strafing, dodging )
        // if target is out of combat range switch to pursue target
        // if target is no longer present return to idle

        [Header("Attacks")] 
        public List<AICharacterAttackAction> aiCharacterAttacks; // List of all possible attacks
        protected List<AICharacterAttackAction> potentialAttacks; // List of all attacks created for this state

        private AICharacterAttackAction chosenAttack;
        private AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo; // Can Perform Combos
        [SerializeField] protected int chanceToPerformCombo = 20; // Chance To Perform Combo
        protected bool hasRolledForCombo = false;  // if combo has been rolled for

        [Header("Engagement Distance")]
        public float maxEngageDistance = 5f; // Distance from target before pursuing

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction) return this;
            if (!aiCharacter.navMeshAgent.enabled) aiCharacter.navMeshAgent.enabled = true;

            // if you want the character to face towards the target
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }

            // Rotate To Face Target

            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // Request attack
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attackStance.currentAttack = chosenAttack;
                // Roll For Combo Chance
                return SwitchState(aiCharacter, aiCharacter.attackStance);
            }

            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maxEngageDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            var path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            if (path.status == NavMeshPathStatus.PathComplete) aiCharacter.navMeshAgent.SetPath(path);

            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();

            // Sort Through all Possible Attacks
            foreach (var potentialAttack in aiCharacterAttacks)
            {
                // If we are too close
                if (potentialAttack.minimumAttackDistance >
                    aiCharacter.aiCharacterCombatManager.distanceFromTarget) continue;
                // if we are too far
                if (potentialAttack.maximumAttackDistance <
                    aiCharacter.aiCharacterCombatManager.distanceFromTarget) continue;
                // If we are outside the minimum angle
                if (potentialAttack.minimumAttackAngle >
                    aiCharacter.aiCharacterCombatManager.viewableAngle) continue;
                // If we are outside the maximum angle
                if (potentialAttack.maximumAttackDistance <
                    aiCharacter.aiCharacterCombatManager.viewableAngle) continue;
                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0) return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }

            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight)
                {
                    // This is the attack
                    chosenAttack = previousAttack = attack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            var randomPercentage = Random.Range(0, 100);
            return randomPercentage < outcomeChance;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;
            hasRolledForCombo = false;
        }

    }
}