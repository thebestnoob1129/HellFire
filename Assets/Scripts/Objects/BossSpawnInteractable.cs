using UnityEngine;
using Unity.Netcode;

namespace CFS
{
    public class BossSpawnInteractable : NetworkBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private GameObject[] spawnObjects;

        [Header("ID")] 
        public int bossSpawnID;

        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnIsActiveChanged(false, isActive.Value);
            isActive.OnValueChanged += OnIsActiveChanged;

            WorldObjectManager.Instance.AddBossSpawnerToList(this);
        }

        private void OnIsActiveChanged(bool previousValue, bool newValue)
        {

            if (isActive.Value)
            {
                foreach (var obj in spawnObjects)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (var obj in spawnObjects)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}