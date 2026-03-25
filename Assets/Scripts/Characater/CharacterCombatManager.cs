using System;
using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class CharacterCombatManager : MonoBehaviour
    {

        protected CharacterManager character;

        [Header("Attack Target")]
        public Transform head;
        public CharacterManager currentTarget;

        [Header("Attack Type")]
        public AttackType currentAttackType;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            head = GetComponentInChildren<LockOnTransform>().transform;
        }

        public virtual void Start()
        {

        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    // Notify Network Of New Target
                    character.characterNetworkManager.currentLockOnTargetID.Value =
                        newTarget.GetComponent<NetworkObject>().NetworkObjectId;

                }
                else
                {
                    currentTarget = null;
                }
            }
        }

    }
}