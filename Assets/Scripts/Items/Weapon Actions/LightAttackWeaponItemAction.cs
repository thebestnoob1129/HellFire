using Unity.Netcode;
using UnityEngine;

namespace CFS
{

    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {

        [SerializeField] private string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] private string light_Attack_02 = "Main_Light_Attack_02";
        [SerializeField] private string light_Attack_03 = "Main_Light_Attack_03";
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction,
            WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
            
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

            // Check For Stops

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;

            // No Ariel Attack
            if (!playerPerformingAction.isGrounded) return;

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);

        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // If we are attacking and are able to perform combo, do combo
            if (playerPerformingAction.playerCombatManager.canComboWithMainWeapon &&
                playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainWeapon = false;

                // perform attack based on previous attack
                if (playerPerformingAction.characterCombatManager.lastAttackPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else if (playerPerformingAction.characterCombatManager.lastAttackPerformed == light_Attack_02)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack03, light_Attack_03, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }

            }
            // otherwise perform regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }

        }
    }
}