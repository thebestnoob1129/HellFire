using CFS;
using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_Attack_01 = "Main_Heavy_Attack_01";

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
            if (playerPerformingAction.isPerformingAction) return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true);
        }
    }
}