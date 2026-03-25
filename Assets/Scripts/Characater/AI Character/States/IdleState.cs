using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "A.I/State/Idle")]
    public class IdleState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {

            if (aiCharacter.characterCombatManager.currentTarget != null)
            {
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }
            else
            {
                // return this state to search for a target
                aiCharacter.aiCharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);
                return this;
            }
        }
    }
}