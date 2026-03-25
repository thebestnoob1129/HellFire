using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;
        public override void ProcessEffect(CharacterManager character)
        {
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            // Compare the base stamina damage against other effects/modifiers
            // Change the value before subtracting / adding it
            // Play sound FX / VFX during effect

            if (character.IsOwner)
            {
                Debug.Log($"Applying stamina damage: {staminaDamage}");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }

        }
    }
}
