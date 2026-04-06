using UnityEngine;

namespace CFS
{
    // REFERENCE OF DATA FOR EVERY SAVE FILE

    [System.Serializable]
    public class CharacterSaveData
    {
        public string fileName = "player.Data";

        [Header("Scene Index")]
        public int sceneIndex = 1;
        public string characterName = "";

        [Header("Time Played")]
        public float secondsPlayed;

        [Header("Resources")]
        public int currentHealth = 100;
        public float currentStamina = 100;

        [Header("Discoverables")]
        public SerializableDictionary<int, bool> journalsFound; 
        public SerializableDictionary<int, bool> evidenceFound; 

        [Header("Levels")]
        public SerializableDictionary<int, bool> levelsCompleted; 

        [Header("Bosses")]
        public SerializableDictionary<int, bool> bossesAwakened; 
        public SerializableDictionary<int, bool> bossesDefeated;

        public CharacterSaveData()
        {
            journalsFound = new SerializableDictionary<int, bool>();
            evidenceFound = new SerializableDictionary<int, bool>();
            levelsCompleted = new SerializableDictionary<int, bool>();

            bossesAwakened = new SerializableDictionary<int, bool>();
            //bossesAwakened.TryAdd(0, false);
            bossesDefeated = new SerializableDictionary<int, bool>();
            //bossesDefeated.TryAdd(0, false);
        }
    }
}