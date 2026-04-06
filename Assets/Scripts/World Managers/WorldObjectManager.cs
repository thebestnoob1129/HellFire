using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager Instance { get; private set; }

        [Header("Objects")]
        [SerializeField] private List<NetworkObjectSpawner> networkSpawners;
        [SerializeField] private List<GameObject> spawnedObjects;

        [Header("Boss Spawns")]
        public List<BossSpawnInteractable> bossSpawn { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void SpawnObject(NetworkObjectSpawner spawner)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            networkSpawners.Add(spawner);
            spawner.AttemptToSpawnCharacter();
        }

        public void AddBossSpawnerToList(BossSpawnInteractable spawner)
        {
            if (!bossSpawn.Contains(spawner))
            {
                bossSpawn.Add(spawner);
            }
        }
        public void RemoveBossSpawnerFromList(BossSpawnInteractable spawner)
        {
            if (bossSpawn.Contains(spawner))
            {
                bossSpawn.Remove(spawner);
            }
        }

        // Create a object Script that will hold all logic for fog walls
        // Spawn in fogwalls as network objects during the start of the game
        // Create general object spawner script and prefab
        // When the fog walls are spawn, add to list of wall objects
        // Use list to grab correct fog wall based on boss

    }
}