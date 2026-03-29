using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;

            playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;

        }
    }
}