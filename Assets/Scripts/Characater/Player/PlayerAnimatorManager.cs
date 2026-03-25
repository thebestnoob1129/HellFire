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
	}
}