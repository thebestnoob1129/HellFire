using UnityEngine;
using UnityEngine.SceneManagement;

namespace CFS
{
    // REFERENCE OF DATA FOR EVERY SAVE FILE

    [System.Serializable]
    public class CharacterSaveData
    {

        public string fileName = "no_slot";
        public CharacterSlot slot = CharacterSlot.NO_SLOT;

        [Header("Scene Index")]
        public int sceneIndex = 1;
        public string characterName = "";

        [Header("Time Played")]
        public float secondsPlayed;

        [Header("Stats")]
        public int vitality = 1;
        public int endurance = 1;

        [Header("Resources")]
        public int currentHealth = 100;
        public float currentStamina = 100;

        [Header("Level Completed")]
        public LevelData[] levelData = new LevelData[30];

        public struct LevelData
        {
            // save data by scene name
            public string name;
            public float timePlayed;
            public bool isCompleted;
            public bool isUnlocked;

            public string GetLevelBySceneName(string scene)
            {
                name = SceneManager.GetSceneByName(scene).name;
                return name;
            }

        }

        [Header("Bosses")]
        public SerializableDictionary<int, bool> bossesAwakened; 
        public SerializableDictionary<int, bool> bossesDefeated;

        public CharacterSaveData()
        {
            bossesAwakened = new SerializableDictionary<int, bool>();
            //bossesAwakened.TryAdd(0, false);
            bossesDefeated = new SerializableDictionary<int, bool>();
            //bossesDefeated.TryAdd(0, false);
        }
    }
}