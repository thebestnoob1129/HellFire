using Unity.Netcode;
using UnityEngine;

namespace CFS
{
    public class NetworkObjectSpawner : MonoBehaviour
    {
        [Header("Object")]
        [SerializeField] private GameObject networkGameObject;
        [SerializeField] private GameObject instantiatedCharacter;


        private void Start()
        {
            WorldObjectManager.Instance.SpawnObject(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacter()
        {
            if (networkGameObject != null)
            {
                instantiatedCharacter = Instantiate(networkGameObject);
                instantiatedCharacter.transform.position = transform.position;
                instantiatedCharacter.transform.rotation = transform.rotation;

                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}