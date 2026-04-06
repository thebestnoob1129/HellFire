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
            if (playerPerformingAction.isPerformingAction) return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            
        }
    }
}