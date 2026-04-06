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


        private void Awake()
        {
            Instance = this;
        }

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartGame()
        {
            WorldSaveGameManager.Instance.StartGame();
        }

        public void NewGame()
        {
            WorldSaveGameManager.Instance.NewGame();
        }

        public void LoadGame()
        {
            WorldSaveGameManager.Instance.LoadGame();
        }

        public void DisplayPopUp(string title, string description)
        {
            popUpMenu.SetActive(true);
            popUpTitle.text = title;
            popUpDescription.text = description;
            popUpConfirm.GetComponentInParent<Button>().Select();

        }

        #region Character Slot

        public void DeleteCharacterSlot()
        {
            popUpConfirm.GetComponentInParent<Button>().onClick.RemoveListener(DeleteCharacterSlot);
            WorldSaveGameManager.Instance.DeleteGame();

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