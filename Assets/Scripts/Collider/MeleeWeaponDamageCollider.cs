using System.Data.Common;
using UnityEngine;

namespace CFS
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifiers")]
        public float lightAttack01Modifier;
        public float lightAttack02Modifier;
        public float heavyAttack01Modifier;
        public float heavyAttack02Modifier;
        public float chargedAttack01Modifier;

        protected override void Awake()
        {
            base.Awake();

            damageCollider.enabled = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget == null || damageTarget == characterCausingDamage) return;

            /*
            if you want to search on both the damageable collider and the character controller
            if (damageTarget == null)
            {
                damageTarget = other.GetComponent<CharacterManager>();
            }
            */


            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // Check for friendly fire

                // Check if target is blocking

                // Check for invulnerability frames

                // damage
                DamageTarget(damageTarget);
            }
        }

        protected override void DamageTarget(CharacterManager target)
        {
            // We don't want to damage the same target more than once per attack

            if (charactersDamaged.Contains(target)) return;

            charactersDamaged.Add(target);

            var damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.standardDamage = standardDamage;
            damageEffect.strikeDamage = strikeDamage;
            damageEffect.slashDamage = slashDamage;
            damageEffect.pierceDamage = pierceDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.iceDamage = iceDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, target.transform.forward, Vector3.up);


            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifier(lightAttack01Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifier(heavyAttack01Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    ApplyAttackDamageModifier(chargedAttack01Modifier, damageEffect);
                    break;
                default:
                    break;
            }

            if (characterCausingDamage.IsOwner)
            {
                // Send a Damage Request Across Network
                target.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    target.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.holyDamage,
                    damageEffect.poiseDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }

        }

        private void ApplyAttackDamageModifier(float modifier, TakeDamageEffect damageEffect)
        {
            damageEffect.physicalDamage *= modifier;
            damageEffect.standardDamage *= modifier;
            damageEffect.strikeDamage *= modifier;
            damageEffect.slashDamage *= modifier;
            damageEffect.pierceDamage *= modifier;
            damageEffect.magicDamage *= modifier;
            damageEffect.fireDamage *= modifier;
            damageEffect.iceDamage *= modifier;
            damageEffect.lightningDamage *= modifier;
            damageEffect.holyDamage *= modifier;

            // if attack is fully charged heavy, multiply by full charge modifier

        }

    }
}
