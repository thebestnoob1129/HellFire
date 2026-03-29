using CFS;
using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] private string heavy_Attack_02 = "Main_Heavy_Attack_02";
        [SerializeField] private string heavy_Attack_03 = "Main_Heavy_Attack_03";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction,
            WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.IsOwner) return;

            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

            // Check For Stops

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;

            // No Ariel Attack
            if (!playerPerformingAction.isGrounded) return;

            PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);

        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // If we are attacking and are able to perform combo, do combo
            if (playerPerformingAction.playerCombatManager.canComboWithMainWeapon &&
                playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainWeapon = false;

                // perform attack based on previous attack
                if (playerPerformingAction.characterCombatManager.lastAttackPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else if (playerPerformingAction.characterCombatManager.lastAttackPerformed == heavy_Attack_02)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack03, heavy_Attack_03, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
                }

            }
            // otherwise perform regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
    }
}