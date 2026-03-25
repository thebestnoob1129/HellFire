using UnityEngine;

namespace CFS
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            Debug.Log("Run this state");

            // logic to find player

            // if we found player, persue target player

            // else continue idle state

            return this;
        }

        protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
        {
            ResetStateFlags(aiCharacter);
            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {
            // Reset Any State Flags
        }
    }
}