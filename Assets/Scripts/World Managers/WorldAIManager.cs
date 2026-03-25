using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace CFS
{
    public class WorldAIManager : MonoBehaviour
    {

        public static WorldAIManager Instance { get; private set; }

        [Header("Debug")] [SerializeField] private bool despawnCharacters = false;
        [SerializeField] private bool respawnCharacters = false;

        [Header("Characters")] [SerializeField]
        private GameObject[] aiCharacters;

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

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                // Spawn All A.I. In Scene
                StartCoroutine(WaitForSceneToLoad());
            }
        }

        private void Update()
        {
            if (despawnCharacters)
            {
                despawnCharacters = false;
                DespawnAllCharacters();
            }

            if (respawnCharacters)
            {
                respawnCharacters = false;
                SpawnAllCharacters();
            }
        }

        private IEnumerator WaitForSceneToLoad()
        {
            while (!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }

            SpawnAllCharacters();

        }

        private void SpawnAllCharacters()
        {
            // Spawn Character in Specific Location by Copying Transform Position To Prefab ( Most likely bosses )
            foreach (var character in aiCharacters)
            {
                var instantiatedCharacter = Instantiate(character);
                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
                spawnedCharacters.Add(instantiatedCharacter);
            }
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
            }

            spawnedCharacters.Clear();
        }

        private void DisableAllCharacters(){
        // Disable character game objects, sync disabled status on network
        // Disable game objects for clients upon connecting, if disabled status is true
        // Can be used to disable characters that are far from players to save memory
        }
    }
}