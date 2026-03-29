using UnityEngine;

namespace CFS
{
	public class PlayerAnimatorManager: CharacterAnimatorManager
    {
        private PlayerManager player;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        private void OnAnimatorMove()
        {
            if (player.applyRootMotion)
            {
                player.characterController.Move(player.animator.deltaPosition);
                player.transform.rotation *= player.animator.deltaRotation;
            }
        }

        // Animation Event Calls
        public override void EnableCanDoCombo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainWeapon = true;
            }
            else
            {
                //canComboWithOffHandWeapon = true;
            }
        }
        public override void DisableCanDoCombo()
        {
            player.playerCombatManager.canComboWithMainWeapon = false;
            //canComboWithOffHandWeapon = false;
        }
    }
}