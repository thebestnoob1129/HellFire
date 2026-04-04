using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace CFS
{
    public class WorldAIManager : MonoBehaviour
    {

        public static WorldAIManager Instance { get; private set; }


        [Header("Characters")]
        [SerializeField] private List<AICharacterSpawner> aiCharacterSpawners;
        [SerializeField] private GameObject[] aiCharacters;
        [SerializeField] private List<GameObject> spawnedCharacters;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SpawnAllCharacters()
        {
            // Spawn Character in Specific Location by Copying Transform Position To Prefab ( Most likely bosses )
            foreach (var character in aiCharacterSpawners)
            {
                character.AttemptToSpawnCharacter();
            }
        }

        public void SpawnCharacter(AICharacterSpawner spawner)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            aiCharacterSpawners.Add(spawner);
            spawner.AttemptToSpawnCharacter();
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
            }

            spawnedCharacters.Clear();
        }

    }
}