using System;
using System.Linq;
using UnityEngine;

namespace CFS
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance { get; private set; }

        [Serializable]
        public struct Wave
        {
            public GameObject[] entity;
            public int enemyCount;
            public bool isFinished;
        }

        [Header("Wave Details")] public Wave[] roundWaves = new Wave[3];
        private int waveNumber;
        [SerializeField] private bool isPaused;

        [Header("Spawn Details")] public Transform[] spawnPoints;
        private bool spawnDelay;

        [Header("Entity Details")] public CharacterManager[] entities;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }

            spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn").Select(x => x.transform).ToArray();
            // Send Level Data To Player
        }

        private void FixedUpdate()
        {

            // Check if the current wave is cleared
            if (entities.Length <= 0 && roundWaves[waveNumber].enemyCount <= 0)
            {
                // Mark the wave as finished
                roundWaves[waveNumber].isFinished = true;

                if (isPaused)
                {
                    return;
                }

                SpawnWave(waveNumber + 1);
            }

            if (isPaused)
            {
                return;
            }

            // If all waves are cleared, end the level

            if (waveNumber >= roundWaves.Length - 1 && entities.Length <= 0)
            {
                Debug.Log("Level Cleared!");
                // Implement level completion logic here
            }

        }

        private void SpawnWave(int wave)
        {
            // Check if the wave has enemies to spawn
            if (roundWaves[wave].enemyCount <= 0)
            {
                Debug.LogWarning($"Wave {wave} has no enemies to spawn.");
                roundWaves[wave].isFinished = true;
                return;
            }

            entities = new CharacterManager[roundWaves[wave].enemyCount];

            for (var i = 0; i < roundWaves[wave].enemyCount; i++)
            {
                Invoke(nameof(SpawnEntity), 1f);
            }
        }

        public void StartGame()
        {
            SpawnWave(0);
        }

        private void SpawnEntity()
        {
            var currentWave = roundWaves[waveNumber];
            var spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            var entity = currentWave.entity[UnityEngine.Random.Range(0, currentWave.entity.Length)];

            var ent = Instantiate(entity, spawnPoints[spawnIndex].transform.position, Quaternion.identity);

            // Store the CharacterManager component of the instantiated entity in the entities array
            entities[^currentWave.enemyCount] = ent.GetComponent<CharacterManager>();
            currentWave.enemyCount--;
            spawnDelay = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            foreach (var spawn in spawnPoints)
            {
                Gizmos.DrawSphere(spawn.position, 0.5f);
            }

        }
    }
}