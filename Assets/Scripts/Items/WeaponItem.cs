using UnityEngine;

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
        public float lightAttack01Modifier = 1.1f;
        public float heavyAttack01Modifier = 1.5f;
        public float chargedAttack01Modifier = 1.75f;
        public float criticalAttack01Modifier = 2f;
        // Critical Damage Modifier

        [Header("Stamina Cost Modifiers")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostModifier = 0.9f;
        // Running attack stamina cost modifier
        // heavy attack stamina cost modifier

        // Item Based Actions ( RB, LT,)
        [Header("Actions")]
        public WeaponItemAction ohRBAction; // One-Handed Right Bumper Action
        public WeaponItemAction ohRTAction; // One-Handed Right Trigger Action

        // Ash of war

        // Blocking Sounds

    }
}