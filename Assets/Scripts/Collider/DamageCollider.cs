using System;
using System.Collections.Generic;
using UnityEngine;

namespace CFS
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        [SerializeField] protected Collider damageCollider;

        [Header("Damage")]
        public float physicalDamage;
        public float standardDamage;
        public float strikeDamage;
        public float slashDamage;
        public float pierceDamage;

        public float magicDamage;
        public float fireDamage;
        public float iceDamage;
        public float lightningDamage;
        public float holyDamage;
        public float poiseDamage;

        [Header("Contact Point")]
        protected Vector3 contactPoint; // The point where the character was hit for blood fx

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>(); // Prevents multiple attacks per attack

        protected virtual void Awake()
        {
            if (!damageCollider) damageCollider = GetComponent<Collider>();
            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var damageTarget = other.GetComponentInParent<CharacterManager>();
            
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

        protected virtual void DamageTarget(CharacterManager target)
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
            damageEffect.contactPoint = contactPoint;

            target.characterEffectsManager.ProcessInstantEffect(damageEffect);


        }

        public virtual void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear(); // Reset Characters that have been hit
        }
    }
}