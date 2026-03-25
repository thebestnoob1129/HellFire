using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CFS
{
    public class WorldGameSessionManager : MonoBehaviour
    {

        public static WorldGameSessionManager Instance;

        [Header("Active Players In Session")]
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayerToActivePlayerList(PlayerManager player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            // Check list for null spots
            for (var i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
        public void RemovePlayerToActivePlayerList(PlayerManager player)
        {
            // Check List For Player
            if (players.Contains(player))
            {
                players.Remove(player);
            }

            // Check list for null spots
            for (var i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }

        }

    }
}