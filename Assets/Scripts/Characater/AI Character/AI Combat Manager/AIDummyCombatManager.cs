using UnityEngine;

namespace CFS
{
    public class AIDummyCombatManager : AICharacterCombatManager
    {
        // Reference: EP40
        [SerializeField] private AICharacterManager aiCharacter;

        [Header("Damage Colliders")]
        [SerializeField] private HandDamageCollider rightHandCollider;
        [SerializeField] private HandDamageCollider leftHandCollider;

        [Header("Damage")]
        [SerializeField] private int baseDamage = 20;
        [SerializeField] private float attack01DamageModifier = 1.0f;
        [SerializeField] private float attack02DamageModifier = 1.3f;

        public void SetAttack01Damage()
        {
            rightHandCollider.physicalDamage = baseDamage * attack01DamageModifier;
            leftHandCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }
        public void SetAttack02Damage()
        {
            rightHandCollider.physicalDamage = baseDamage * attack02DamageModifier;
            leftHandCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }

        public void OpenRightHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            rightHandCollider.EnableDamageCollider();
        }

        public void OpenLeftHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            leftHandCollider.EnableDamageCollider();
        }

        public void CloseRightHandDamageCollider()
        {
            rightHandCollider.DisableDamageCollider();
        }

        public void CloseLeftHandDamageCollider()
        {
            leftHandCollider.DisableDamageCollider();
        }

    }
}