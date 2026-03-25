using CFS;
using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_attack_01 = "Main_Heavy_Attack_01";

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

            if (playerPerformingAction.playerNetworkManager.isUsingRightHand.Value)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_attack_01, true);

                return;
            }

            if (playerPerformingAction.playerNetworkManager.isUsingLeftHand.Value)
            {

                return;
            }
        }
    }
}