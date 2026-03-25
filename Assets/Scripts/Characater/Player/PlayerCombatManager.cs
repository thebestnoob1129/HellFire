using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        private PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (player.IsOwner)
            {
                // Perform The Action
                weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

                // Perform Action Over Server
                player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
            }
        }

        // Go To Animation File To add Function To Run At Certain Frame
        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner) return;

            if (!currentWeaponBeingUsed) return;

            float staminaDeducted = 0f;

            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.lightAttackStaminaCostModifier;
                    break;
                default:
                    break;
            }
            
            Debug.Log("Stamina Deducted: " + staminaDeducted, gameObject);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);

        }

    }
}