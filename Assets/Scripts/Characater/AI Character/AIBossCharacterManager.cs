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
        [SerializeField] List<BossSpawnInteractable> bossSpawn;

        // When spawned check if defeated
        // If save file doesn't contain ID add it
        // if boss has been defeated add it, disable game object
        // if boss has not been defeated allow object to continue to be active

        [Header("Debug")] public bool wakeBossUp;

        protected override void Update()
        {
            base.Update();
            if (wakeBossUp)
            {
                WakeBoss();
                wakeBossUp = false;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();



            if (IsServer)
            {
                // Locate Boss Spawn
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
                    isAwakened = WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened[bossID];
                }

                StartCoroutine(GetBossSpawn());

                // If awakened enable boss spawn
                if (isAwakened)
                {
                    foreach (var wall in bossSpawn)
                    {
                        wall.isActive.Value = true;
                    }
                }

                // if defeated disable boss spawn and boss 
                if (isDefeated)
                {
                    // Sets game object to inactive if boss has been defeated
                    aiCharacterNetworkManager.isActive.Value = false;
                    foreach (var wall in bossSpawn)
                    {
                        wall.isActive.Value = false;
                    }
                }


                // Either match boss spawn ID to boss data or add new boss data with ID from boss spawn
            }
        }

        private IEnumerator GetBossSpawn()
        {
            while (WorldObjectManager.Instance.bossSpawn.Count == 0)
            {
                yield return new WaitForSeconds(1f);
            }

            bossSpawn = new List<BossSpawnInteractable>();

            foreach (var wall in WorldObjectManager.Instance.bossSpawn)
            {
                if (wall.bossSpawnID == bossID)
                {
                    bossSpawn.Add(wall);
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
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // load the data if it already exists
                else
                {
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.Instance.SaveGame();

            }
            // Play Death SFX

            // Play Death VFX

            yield return new WaitForSeconds(5f);

            // Award or Finish any required objectives

            // Disable Character
        }

        public void WakeBoss()
        {
            isAwakened = true;

            if (!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            for (int i = 0; i < bossSpawn.Count; i++)
            {
                bossSpawn[i].isActive.Value = true;
            }

        }
    }
}