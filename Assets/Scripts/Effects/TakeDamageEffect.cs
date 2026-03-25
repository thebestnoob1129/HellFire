using System;
using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // If the damage is caused by another character store here

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

        [Header("Final Damage")]
        private int finalDamageDealt;
        // Build Ups
        // build up effect amount

        [Header("Poise")] public float poiseDamage = 0;
        public bool poiseIsBroken; // If a character poise is broken, play stun animation

        [Header("Animation")] public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation;
        public string damageAnimation;

        [Header("Sound Fx")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSFX; // Used on top of regular damage sfx, if elemental damage is being dealt

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; // Used to determine what damage animation to play
        public Vector3 contactPoint; // The point where the character was hit for blood fx

        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            // No Additional Effects, just damage
            if (character.isDead.Value) return;

            // Check For "Invulnerability" Status Effects Here

            CalculateDamage(character);

            // Check Which direction damage came from

            PlayDirectionBasedDamageAnimation(character);

            // Check For build Ups (poison, Bleed)

            PlayDamageSFX(character);

            PlayDamageVFX(character);

            // If character is A.I, Check for new target if character causing damage is present

        }

        private void CalculateDamage(CharacterManager character)
        {
            // Calculate Final Damage Here, Apply To Character Stats Manager

            if (!character.IsOwner) return;
            if (characterCausingDamage != null)
            {
                // Check For Damage Modifiers And Modify Base Damage (Physical Damage buff, elemental damage buff, etc)
            }

            // Check character for Flat defenses and subtract them

            // Check character for Armor Absorptions and reduce damage by percentage from damage

            // Add all damage types together to get final damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + standardDamage + strikeDamage + slashDamage + pierceDamage +
                                              magicDamage + fireDamage + iceDamage + lightningDamage + holyDamage);

            if (finalDamageDealt < 0) finalDamageDealt = 1;

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            // Calculate poise damage and check if poise is broken
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            // if we have elemental damage, play elemental particles

            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }

        private void PlayDamageSFX(CharacterManager character)
        {
            var physicalDamageSFX = WorldSoundFXManager.Instance.ChooseRandomSFX(WorldSoundFXManager.Instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);

            // if elemental damage, play elemental sound
        }

        private void PlayDirectionBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner) return;

            if (character.isDead.Value) return;

            // Calculate if poise is broken
            poiseIsBroken = true;

            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forwardMediumDamage);
                //character.characterAnimatorManager.PlayTargetActionAnimation(character.characterAnimatorManager.hitForwardMedium01, true);
            }
            else if (angleHitFrom >= -145 && angleHitFrom <= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forwardMediumDamage);
                //character.characterAnimatorManager.PlayTargetActionAnimation(character.characterAnimatorManager.hitForwardMedium01, true);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backwardMediumDamage);
                //character.characterAnimatorManager.PlayTargetActionAnimation(character.characterAnimatorManager.hitBackwardMedium01, true);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.leftMediumDamage);
                //character.characterAnimatorManager.PlayTargetActionAnimation(character.characterAnimatorManager.hitLeftMedium01, true);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.rightMediumDamage);
                //character.characterAnimatorManager.PlayTargetActionAnimation(character.characterAnimatorManager.hitRightMedium01, true);
            }

            // Play Stun Animation
            if (poiseIsBroken)
            {
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }

        }

    }
}