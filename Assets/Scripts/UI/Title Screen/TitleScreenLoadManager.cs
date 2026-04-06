using System;
using UnityEngine;

namespace CFS
{
    public class TitleScreenLoadManager : MonoBehaviour
    {
        private PlayerControls playerControls;

        [Header("Title Screen Inputs")]
        [SerializeField] private bool deleteCharacterSlot = false;

        private void Update()
        {
            
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.UI.Cancel.performed += ctx => deleteCharacterSlot = true;
            }
            playerControls.Enable();

        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}