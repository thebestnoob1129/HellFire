using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace CFS
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager Instance;
        public PlayerManager player;

        [Header("Menus")]
        [SerializeField] private GameObject titleScreenMainMenu;
        [SerializeField] private GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] private Button loadMenuReturnButton;
        [SerializeField] private Button mainMenuLoadGameButton;
        [SerializeField] private Button mainMenuNewGameButton;

        [Header("Pop Ups")]// CREATE DYNAMIC POP UP FUNCTION, CREATE AND ACTIVATE POP UPS FROM 1 FUNCTION
        [SerializeField] private GameObject popUpMenu;
        [SerializeField] private TMP_Text popUpTitle;
        [SerializeField] private TMP_Text popUpDescription;
        [SerializeField] private TMP_Text popUpConfirm;
        [SerializeField] private TMP_Text popUpExit;

        [Header("Save Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

        private void Awake()
        {
            Instance = this;
        }

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.Instance.AttemptCreateNewGame();
        }

        public void ContinueGame()
        {
            WorldSaveGameManager.Instance.LoadGame((CharacterSlot)PlayerPrefs.GetInt("LastSaveUsed", 0));
        }

        public void DisplayPopUp(string title, string description)
        {
            popUpMenu.SetActive(true);
            popUpTitle.text = title;
            popUpDescription.text = description;
            popUpConfirm.GetComponentInParent<Button>().Select();

        }

        #region Character Slot
        public void SelectCharacterSlot(CharacterSlot slot)
        {
            currentSelectedSlot = slot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        public void AttemptToDeleteCharacterSlot()
        {
            if (currentSelectedSlot == CharacterSlot.NO_SLOT) return;

            DisplayPopUp("Delete Character?", "You are going to delete all character save data.");
            popUpConfirm.GetComponentInParent<Button>().onClick.AddListener(DeleteCharacterSlot);

        }

        public void DeleteCharacterSlot()
        {
            popUpConfirm.GetComponentInParent<Button>().onClick.RemoveListener(DeleteCharacterSlot);
            WorldSaveGameManager.Instance.DeleteGame(currentSelectedSlot);

            // WE RESET TO REFRESH ALL CHARACTER SLOTS
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            loadMenuReturnButton.Select();
        }
        #endregion

        public void Exit()
        {
            Application.Quit();
        }

    }
}