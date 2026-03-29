using UnityEngine;

// Equip Animation: EP20 31:10
// Drain Stamina / Open Close Stamina: EP31: 16:00

namespace CFS
{
    public class WeaponItem : Item
    {
        // Animator controller override ( cahnge attack animations based on weapon you are holding)

        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        public int strReq;
        public int dexReq, intReq, magicReq;

        [Header("Weapon Base Damage")]
        public int physicalDamage;
        public int magicDamage, fireDamage, holyDamage, lightningDamage;

        // Weapon Guard Absorbions

        [Header("Weapon Poise")]
        public float poiseDamage = 10;
        // Offensive Poise Bonus

        [Header("Attack Modifier")]
        // Weapon Modifiers
        public float lightAttack01Modifier = 1.0f;
        public float lightAttack02Modifier = 1.1f;
        public float lightAttack03Modifier = 1.1f;
        public float heavyAttack01Modifier = 1.5f;
        public float heavyAttack02Modifier = 1.7f;
        public float heavyAttack03Modifier = 1.7f;
        public float chargedAttack01Modifier = 1.75f;
        public float chargedAttack02Modifier = 2.3f;
        public float criticalAttack01Modifier = 2f;

        [Header("Stamina Cost Modifiers")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostModifier = 0.9f;
        public float heavyAttackStaminaCostModifier = 1.5f;
        // Running attack stamina cost modifier

        // Item Based Actions ( RB, LT)
        [Header("Actions")]
        public WeaponItemAction attackAction; // One-Handed Right Bumper Action
        public WeaponItemAction heavyAction; // One-Handed Right Bumper Action
        public WeaponItemAction chargeAction; // One-Handed Right Trigger Action

        [Header("Whooshes")] 
        public AudioClip[] whooshes;

        // Ash of war

        // Blocking Sounds

    }
}