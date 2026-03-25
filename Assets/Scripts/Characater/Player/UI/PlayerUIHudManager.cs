using System;
using UnityEngine;
using UnityEngine.UI;

namespace CFS
{
	public class PlayerUIHudManager: MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] private UI_StatBar healthBar;
        [SerializeField] private UI_StatBar staminaBar;
        [SerializeField] private UI_StatBar experienceBar;

        [Header("Quick Slots")]
        [SerializeField] private Image rightWeaponQuickSlotIcon;
        [SerializeField] private Image leftWeaponQuickSlotIcon;

        [SerializeField] private GameObject interactor;

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }
        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }
        public void SetNewExperienceValue(float oldValue, float newValue)
        {
            experienceBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxHealthValue(int maxStamina)
        {
            healthBar.SetMaxStat(maxStamina);
        }
        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }
        public void SetMaxExperienceValue(int maxStamina)
        {
            experienceBar.SetMaxStat(maxStamina);
        }

        public void SetRightWeaponIcon(int weaponID)
        {
            var weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // Check For Item Requirements & use weapon requirements

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
        }
        public void SetLeftWeaponIcon(int weaponID)
        {
            var weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            // Check For Item Requirements & use weapon requirements

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
        }
        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(true);
            healthBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(true);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (PlayerCamera.Instance.player) interactor.SetActive(PlayerCamera.Instance.player.canInteract);
        }
    }
}