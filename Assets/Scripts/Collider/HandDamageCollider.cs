using UnityEngine;

namespace CFS
{
    [RequireComponent(typeof(SphereCollider))]
    public class HandDamageCollider : DamageCollider
    {
        [SerializeField] private AICharacterManager aiCharacterCausingDamage;

        protected override void Awake()
        {
            base.Awake();

            damageCollider = GetComponent<Collider>();
            damageCollider.enabled = false;
            aiCharacterCausingDamage = GetComponentInParent<AICharacterManager>();
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
            damageEffect.angleHitFrom = Vector3.SignedAngle(aiCharacterCausingDamage.transform.forward, target.transform.forward, Vector3.up);

            // Option 01:
            // Apply Damage On Hit On The Host Side
            /*
            if (aiCharacterCausingDamage.IsOwner)
            {
                // Send a Damage Request Across Network
                target.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    target.NetworkObjectId,
                    aiCharacterCausingDamage.NetworkObjectId,
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
            */
            // Option 02:
            // Apply Damage On Hit On The Host Side
            if (aiCharacterCausingDamage.IsOwner)
            {
                // Send a Damage Request Across Network
                target.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    target.NetworkObjectId,
                    aiCharacterCausingDamage.NetworkObjectId,
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


    }
}