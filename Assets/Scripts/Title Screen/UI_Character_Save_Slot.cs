using UnityEngine;
using TMPro;

namespace CFS
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        private SaveGameDataWriter saveWriter;

        [Header("Game Slot")]
        [SerializeField] private CharacterSlot characterSlot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlot();
        }

        private void LoadSaveSlot()
        {
            saveWriter = new SaveGameDataWriter()
            {
                saveDataDirectory = Application.persistentDataPath,
                saveFileName = "slot_" + (int)characterSlot
            };

            if (saveWriter.CheckIfFileExists(saveWriter.saveFileName))
            {
                characterName.text = WorldSaveGameManager.Instance.characterSlots[(int)characterSlot].characterName ?? "No Name";
                timePlayed.text = "Time Played: " + WorldSaveGameManager.Instance.characterSlots[(int)characterSlot].secondsPlayed;
            }
            else
            {
                characterName.text = "+";
                timePlayed.text = " Create New Character ";
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.Instance.currentCharacterSlot = characterSlot;
            WorldSaveGameManager.Instance.LoadGame(characterSlot);
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.Instance.SelectCharacterSlot(characterSlot);
        }

    }
}