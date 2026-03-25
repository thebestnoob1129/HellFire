using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CFS
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance { get; private set; }

        [HideInInspector] public PlayerManager player;
        public int defaultSlot = 9;

        [Header("Save/Load")]
        public bool saveGame;
        public bool loadGame;

        [Header("World Scene Index")]
        [SerializeField] private int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveGameDataWriter saveDataWriter;

        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlot;
        public CharacterSaveData currentCharacterData;

        [Header("Character Slots")] // Create Into Dynamic Ray
        public CharacterSaveData[] characterSlots = new CharacterSaveData[11];
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadAllCharacterProfiles();
            currentCharacterSlot = (CharacterSlot)PlayerPrefs.GetInt("LastSaveUsed", 0);
            currentCharacterData = characterSlots[(int)currentCharacterSlot];
            
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame(currentCharacterSlot);
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame(currentCharacterSlot);
            }
        }

        private void LoadAllCharacterProfiles()
        {
            // Change back to directory when i add dynamic character creation
            for (var i = 0; i < 10; i++)
            {
                // Dynamic File names for Multiple Characters
                // Add A Customization Screen
                saveDataWriter = new SaveGameDataWriter
                {
                    saveDataDirectory = Application.persistentDataPath,
                    saveFileName = "slot_" + i
                };

                if (!saveDataWriter.CheckIfFileExists("slot_"+i))
                {
                    //Debug.LogWarning("File Does Not Exist: " + i);
                    characterSlots[i] = new CharacterSaveData()
                    {
                        fileName = "slot_" + i,
                        slot = (CharacterSlot)i
                    };
                }
                else
                {
                    characterSlots[i] = saveDataWriter.LoadSaveFile();
                }
            }
        }

        public void AttemptCreateNewGame()
        {
            currentCharacterData = new CharacterSaveData();
            saveDataWriter.saveDataDirectory = Application.persistentDataPath;

            // CHECK TO SEE IF SAVE FILE CONTAINS SLOT, MAKE DYNAMIC
            saveDataWriter.saveFileName = PlayerPrefs.GetString("LastSlotUsed") ?? "slot_0";
            if (!saveDataWriter.CheckIfFileExists(saveDataWriter.saveFileName))
            {
                // IF PROFILE IS NOT TAKEN, CREATE NEW FILE
                // Character Will be created and save in game 

                // Get Next Available Slot
                currentCharacterSlot = CharacterSlot.NO_SLOT;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            TitleScreenManager.Instance.DisplayPopUp("No Free Slots", "You have the max capacity of slots available: 10.");
        }

        private void NewGame()
        {
            // CREATE NEW CHARACTER DATA
            SaveGame(currentCharacterSlot);
            StartCoroutine(LoadWorldScene());
        }

        public void LoadGame(CharacterSlot slot)
        {
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = characterSlots[(int)slot].fileName,
            };
            currentCharacterData = saveDataWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame(CharacterSlot slot)
        {

            var slotIndex = (int)slot;
            slotIndex = Mathf.Clamp(slotIndex, 0, 9);

            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = characterSlots[slotIndex].fileName
            };

            // PASS PLAYER INFO FROM GAME TO FILE
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // WRITE INTO TO FILE
            saveDataWriter.CreateNewFile(currentCharacterData);
        }

        public void DeleteGame(CharacterSlot slot)
        {
            // CHOOSE FILE BASED ON GAME
            
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = characterSlots[(int)slot].fileName
            };

            saveDataWriter.DeleteSaveFile();
        }


        public IEnumerator LoadWorldScene()
        {
            
            // MULTIPLE SCENES IN PROJECT
            //var loadOperation = SceneManager.LoadSceneAsync(currentCharacterData != null ? currentCharacterData.sceneIndex : worldSceneIndex);
            var loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            player.LoadGameDataToCurrentCharacterData(ref currentCharacterData);

            yield return loadOperation;
        }

        public int GetWorldSceneIndex() { return worldSceneIndex; }

    }
}