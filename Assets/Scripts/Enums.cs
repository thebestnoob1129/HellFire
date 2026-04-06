using UnityEngine;

namespace CFS
{
	public class Enums: MonoBehaviour
	{

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
        HeavyAttack01,
    }

    public enum CharacterGroup
    {
        Team01, // Friendly
        Team02, // Enemy
    }

}