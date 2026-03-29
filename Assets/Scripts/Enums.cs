using UnityEngine;

namespace CFS
{
	public class Enums: MonoBehaviour
	{


	}

    public enum CharacterSlot
    {
        CharacterSlot01,
        CharacterSlot02,
        CharacterSlot03,
        CharacterSlot04,
        CharacterSlot05,
        CharacterSlot06,
        CharacterSlot07,
        CharacterSlot08,
        CharacterSlot09,
        CharacterSlot10,
        NO_SLOT
    }

    public enum WeaponModelSlot
    {
        RightHand,
        LeftHand,
    }

    // Calculate Damage Based On Type
    public enum AttackType
    {
        LightAttack01,
        LightAttack02,
        LightAttack03,
        HeavyAttack01,
        HeavyAttack02,
        HeavyAttack03,
        ChargedAttack01,
        ChargedAttack02,
        ChargedAttack03,

    }

    public enum CharacterGroup
    {
        Team01, // Friendly
        Team02, // Enemy
    }

}