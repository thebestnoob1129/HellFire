using UnityEngine;

namespace CFS
{
    public class InstantCharacterEffect : ScriptableObject
    {
        [Header("Effect ID")]
        public int instantEffectID;

        public virtual void ProcessEffect(CharacterManager character)
        {
            // Implement the logic for processing the instant effect on the character
            // This method can be overridden by derived classes to provide specific behavior
        }
    }
}