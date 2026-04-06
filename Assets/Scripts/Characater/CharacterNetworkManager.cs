using UnityEngine;
using Unity.Netcode;

namespace CFS
{

    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager character;

        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;

        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Animator")]
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Target")]
        public NetworkVariable<ulong> currentLockOnTargetID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")]
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void CheckHealth(int oldValue, int newValue)
        {
            if (currentHealth.Value <= 0)
            {
                StartCoroutine(character.ProcessDeathEvent());
            }

            if (character.IsOwner)
            {
                if (currentHealth.Value >= maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value;
                }
            }

        }
        /*
        public void OnLockOnTargetIDChanged(ulong oldID, ulong newID)
        {
            if (!IsOwner)
            {
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }

        public void OnIsLockOnChanged(bool oldValue, bool isLockedOn)
        {
            if (!isLockedOn)
            {
                character.characterCombatManager.currentTarget = null;
            }
        }

        public void OnChargingAttackChanged(bool oldValue, bool newValue)
        {
            character.animator.SetBool("isChargingAttack", newValue);
        }

        */
        public void OnIsMovingChanged(bool oldValue, bool newValue)
        {
            character.animator.SetBool("isMoving", isMoving.Value);
        }

        public virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            gameObject.SetActive(isActive.Value);
        }

        // Character Animation

        // A SERVER RPC IS A FUNCTION CALLED FROM A CLIENT, TO A SERVER / HOST
        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // IF THIS CHARACTER IS THE HOST/SERVER, ACTIVATE CLIENT RPC
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        // A CLIENT RPC IS SENT TO ALL CLIENTS PRESENT FROM SERVER
        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // MAKE SURE NOT TO RUN FUNCTION FROM PLAYER WHO SENT IT
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
        
        // Character Attack Animation
        
        // A SERVER RPC IS A FUNCTION CALLED FROM A CLIENT, TO A SERVER / HOST
        [ServerRpc]
        public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // IF THIS CHARACTER IS THE HOST/SERVER, ACTIVATE CLIENT RPC
            if (IsServer)
            {
                PlayAttackActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        // A CLIENT RPC IS SENT TO ALL CLIENTS PRESENT FROM SERVER
        [ClientRpc]
        public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // MAKE SURE NOT TO RUN FUNCTION FROM PLAYER WHO SENT IT
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }

        // Damage Animation
        [ServerRpc]
        public void NotifyTheServerOfCharacterDamageServerRpc(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            if (IsServer)
            {
                NotifyTheServerOfCharacterDamageClientRpc(damagedCharacterID, characterCausingDamageID, physicalDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
            }
        }
        [ClientRpc]
        public void NotifyTheServerOfCharacterDamageClientRpc(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
        public void ProcessCharacterDamageFromServer(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            var damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
            var characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();


            // Damage Character
            var damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);

            damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);

        }


    }
}