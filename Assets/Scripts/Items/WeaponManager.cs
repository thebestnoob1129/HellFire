using UnityEngine;

namespace CFS
{
    public class WeaponManager : MonoBehaviour 
    {
        public MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager wielder, WeaponItem weapon)
        {
            meleeDamageCollider.characterCausingDamage = wielder;
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.fireDamage = weapon.fireDamage;

            meleeDamageCollider.lightAttack01Modifier = weapon.lightAttack01Modifier;
            meleeDamageCollider.lightAttack02Modifier = weapon.lightAttack02Modifier;
            meleeDamageCollider.lightAttack03Modifier = weapon.lightAttack03Modifier;
            meleeDamageCollider.heavyAttack01Modifier = weapon.heavyAttack01Modifier;
            meleeDamageCollider.heavyAttack02Modifier = weapon.heavyAttack02Modifier;
            meleeDamageCollider.heavyAttack03Modifier = weapon.heavyAttack03Modifier;
            meleeDamageCollider.chargedAttack01Modifier = weapon.chargedAttack01Modifier;
            meleeDamageCollider.chargedAttack02Modifier = weapon.chargedAttack02Modifier;
        }
    }
}
