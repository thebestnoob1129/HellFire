using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEditor;

namespace CFS
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        // for all if statements: player.OwnerClientId != NetworkManager.Singleton.LocalClientId

        private PlayerManager player;
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Equipment")] 
        public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandedAction)
        {
            if (rightHandedAction)
            {
                isUsingLeftHand.Value = false;
                isUsingRightHand.Value = true;
            }
            else
            {
                isUsingLeftHand.Value = true;
                isUsingRightHand.Value = false;
            }
            
        }

        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
        {
            var newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentRightWeaponItem = newWeapon;
            player.playerEquipmentManager.LoadRightWeapon();

            if (player.IsOwner)
            {
                PlayerUIManager.Instance.playerHudManager.SetRightWeaponIcon(newID);
            }
        }
        public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
        {
            var newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerInventoryManager.currentLeftWeaponItem = newWeapon;
            player.playerEquipmentManager.LoadLeftWeapon();

            if (player.IsOwner)
            {
                PlayerUIManager.Instance.playerHudManager.SetLeftWeaponIcon(newID);
            }
        }
        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            var newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;
        }

        // Item Actions
        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
        {
            if (IsServer)
            {
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }
        [ClientRpc]
        public void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            // Don't play action again for the calling character ( played locally )
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformWeaponBasedAction(actionID,  weaponID);
            }
        }

        private void PerformWeaponBasedAction(int actionID, int weaponID)
        {
            WeaponItemAction weaponAction = WorldActionManager.Instance.GetWeaponItemAction(actionID);

            if (weaponAction != null)
            {
                weaponAction.AttemptToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(weaponID));
            }
            else
            {
                Debug.LogError("ACTION IS NULL, CANNOT BE PERFORMED");
            }
        }

    }
}