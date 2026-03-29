using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        private CharacterManager character;

        private int vertical, horizontal;

        [Header("Damage Animation")]
        public string lastDamageAnimationPlayed;

        [SerializeField] private string hitForwardMedium01 = "hit_Forward_Medium_01";
        [SerializeField] private string hitForwardMedium02 = "hit_Forward_Medium_02";

        [SerializeField] private string hitBackwardMedium01 = "hit_Backward_Medium_01";
        [SerializeField] private string hitBackwardMedium02 = "hit_Backward_Medium_02";
        
        [SerializeField] private string hitLeftMedium01 = "hit_Left_Medium_01";
        [SerializeField] private string hitLeftMedium02 = "hit_Left_Medium_02";
        
        [SerializeField] private string hitRightMedium01 = "hit_Right_Medium_01";
        [SerializeField] private string hitRightMedium02 = "hit_Right_Medium_02";

        // Hit Animation Variation
        public List<string> forwardMediumDamage = new List<string>();
        public List<string> backwardMediumDamage = new List<string>();
        public List<string> leftMediumDamage = new List<string>();
        public List<string> rightMediumDamage = new List<string>();

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        protected virtual void Start()
        {
            forwardMediumDamage.Add(hitForwardMedium01);
            forwardMediumDamage.Add(hitForwardMedium02);

            backwardMediumDamage.Add(hitBackwardMedium01);
            backwardMediumDamage.Add(hitBackwardMedium02);

            leftMediumDamage.Add(hitLeftMedium01);
            leftMediumDamage.Add(hitLeftMedium02);

            rightMediumDamage.Add(hitRightMedium01);
            rightMediumDamage.Add(hitRightMedium02);
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            var h = horizontalValue;
            var v = verticalValue;
            if (isSprinting)
            {
                v = 2;
            }

            character.animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            var finalList = new List<string>();

            foreach (var item in animationList)
            {
                finalList.Add(item);
            }

            // check if we already played an animation
            finalList.Remove(lastDamageAnimationPlayed);
            
            // Clear Null Entries
            for (var i = finalList.Count - 1; i >= -1; i--)
            {
                if (finalList[i] == null) finalList.RemoveAt(i);
            }

            int randomValue = Random.Range(0, finalList.Count);
            return finalList[randomValue];
        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            Debug.Log("Playing Animation: " + targetAnimation, gameObject);
            
            character.animator.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            // CAN BE USED TO STOP CHARACTER FROM ATTEMPTING ACTION
            // DAMAGE ANIMATIONS CAN STOP PLAYER FROM OTHER ANIMATIONS
            // CHECK THIS BEFORE ATTEMPTING NEW ACTIONS
            character.isPerformingAction = isPerformingAction;

            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);


        }
        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            // Keep Track of Last Attack Performed
            // Keep Track Of Current Attack Type
            // Update Animation Set To Current Weapon Animation
            // Decide if attack can be parried
            // isAttacking flag

            character.characterCombatManager.currentAttackType = attackType;

            // Normal animation
            character.animator.applyRootMotion = applyRootMotion;
            character.characterCombatManager.lastAttackPerformed = targetAnimation;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);


        }


        public virtual void EnableCanDoCombo()
        {
            
        }
        public virtual void DisableCanDoCombo()
        {
            
        }
    }
}