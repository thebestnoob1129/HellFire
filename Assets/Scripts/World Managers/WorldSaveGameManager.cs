using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CFS
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance { get; private set; }

        [HideInInspector] public PlayerManager player;
        public int defaultSlot = 0;

        [Header("Save/Load")]
        public bool saveGame;
        public bool loadGame;

        [Header("World Scene Index")]
        [SerializeField] private int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveGameDataWriter saveDataWriter;

        [Header("Current Character Data")]
        public CharacterSaveData currentCharacterData;

        
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
            
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }

        private void LoadAllCharacterProfiles()
        {
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = "player.Data"
            };

            if (saveDataWriter.CheckIfFileExists("player.Data"))
            {
                currentCharacterData = saveDataWriter.LoadSaveFile();
                return;
            }

            /*
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
            */
        }

        // Start Game = Have game data and load into world

        // New Game = Create new game data and load into world

        public void StartGame()
        {
            if (currentCharacterData == null)
            {
                TitleScreenManager.Instance.DisplayPopUp("No Save Data", "Please create a new game to start playing.");
                NewGame();
                return;
            }
            else
            {
                StartCoroutine(LoadWorldScene());
            }
        }

        public void NewGame()
        {

            // CREATE NEW CHARACTER DATA
            currentCharacterData = new CharacterSaveData
            {
                fileName = saveDataWriter.saveFileName,
            };
            
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = currentCharacterData.fileName
            };
            saveDataWriter.CreateNewFile(currentCharacterData);

            StartCoroutine(LoadWorldScene());
        }

        public void LoadGame()
        {
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = currentCharacterData.fileName,
            };
            currentCharacterData = saveDataWriter.LoadSaveFile();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {

            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = currentCharacterData.fileName
            };

            // PASS PLAYER INFO FROM GAME TO FILE
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            // WRITE INTO TO FILE
            saveDataWriter.CreateNewFile(currentCharacterData);
        }

        public void DeleteGame()
        {
            // CHOOSE FILE BASED ON GAME
            
            saveDataWriter = new SaveGameDataWriter
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = currentCharacterData.fileName
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