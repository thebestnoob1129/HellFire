using System.Globalization;
using UnityEngine;

namespace CFS
{
	public class CharacterStatsManager: MonoBehaviour
    {
        private CharacterManager character;

        [Header("Stamina Regeneration")]
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] private float staminaRegenerationDelay = 2;
        [SerializeField] private int staminaRegenAmount = 2;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner) return;
            if (character.characterNetworkManager.isSprinting.Value) return;
            if (character.isPerformingAction) return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += Mathf.RoundToInt(staminaRegenAmount);
                    }
                }
            }

        }

        public virtual void ResetStaminaRegenTimer(float oldValue, float newValue)
        {
            // ONLY CALL WHEN STAMINA VALUE IS CHANGED
            if (newValue < oldValue)
            {
                staminaRegenerationTimer = 0;
            }
        }
    }
}