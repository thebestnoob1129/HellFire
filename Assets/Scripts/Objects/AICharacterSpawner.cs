using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private GameObject characterGameObject;
        [SerializeField] private GameObject instantiatedCharacter;


        private void Start()
        {
            WorldAIManager.Instance.SpawnCharacter(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedCharacter = Instantiate(characterGameObject);
                instantiatedCharacter.transform.position = transform.position;
                instantiatedCharacter.transform.rotation = transform.rotation;

                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}