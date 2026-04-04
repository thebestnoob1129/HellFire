using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

namespace CFS
{
    public class AIBossCharacterManager : AICharacterManager
    {
        public int bossID = 0;
        [SerializeField] private bool isAwakened = false;
        [SerializeField] private bool isDefeated = false;

        // When spawned check if defeated
        // If save file doesn't contain ID add it
        // if boss has been defeated add it, disable game object
        // if boss has not been defeated allow object to continue to be active

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // Add Information to boss data if it doesn't exist
                if (!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.TryAdd(bossID, false);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, false);
                }
                // load the data if it already exists
                else
                {
                    isDefeated = WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated[bossID];

                    if (isDefeated)
                    {
                        // Sets game object to inactive if boss has been defeated
                        aiCharacterNetworkManager.isActive.Value = false;
                    }
                }
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // Reset Any required Flags

                // If we are not grounded, play aerial death animation

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }

                isDefeated = true;
                Debug.Log("Boss Defeated: " + bossID);
                if (!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    Debug.Log("Boss ID Not Found, Adding Boss ID: " + bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // load the data if it already exists
                else
                {
                    Debug.Log("Boss ID Found, Updating Boss ID: " + bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.Instance.SaveGame(WorldSaveGameManager.Instance.currentCharacterSlot);

            }
            // Play Death SFX

            // Play Death VFX

            yield return new WaitForSeconds(5f);

            // Award or Finish any required objectives

            // Disable Character
        }
    }
}