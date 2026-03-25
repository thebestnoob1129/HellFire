using UnityEngine;
using System.Collections;

namespace CFS
{
	public class PlayerStatsManager: CharacterStatsManager
    {
        private PlayerManager player;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();
            // Stats will be set depending on level of player or character creation menu
            // until then, stats are never calculated
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }

        private void Die()
        {
            Debug.Log("Player is Dead", gameObject);
        }
        private void OnCollisionEnter(Collision other)
        {
            var obj = other.collider.gameObject;

            if (obj.TryGetComponent<Bullet>(out var bullet))
            {
                //OnDamaged(bullet.damage);
            }
        }
    }
}