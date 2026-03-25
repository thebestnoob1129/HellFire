using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "A.I/Actions/Attack")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Combo Action")] 
        [SerializeField] private string attackAnimation;

        [Header("Combo Action")]
        public bool actionHasComboActon = false; // if action has a combo acton ( or check for null)
        public AICharacterAttackAction comboAction; // combo action for this attack

        [Header("Action Values")]
        [SerializeField] private AttackType attackType;
        public bool canBeRepeated = false;
        public int attackWeight = 10; // 0 - 100
        public float actionRecoveryTime = 1.5f; // Time before the character can attack | Dexterity
        public float minimumAttackAngle = -35;
        public float maximumAttackAngle = 35;
        public float minimumAttackDistance = 0;
        public float maximumAttackDistance = 2;


        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);
        }

    }
}