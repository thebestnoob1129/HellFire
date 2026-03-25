using UnityEngine;

// Reference EP:25

namespace CFS
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager player;

        [HideInInspector] public WeaponModelInstantiationSlot rightHandSlot, leftHandSlot;
        public GameObject rightHandWeaponModel, leftHandWeaponModel;

        [SerializeField] private WeaponManager rightWeaponManager;
        [SerializeField] private WeaponManager leftWeaponManager;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();

            InitializeWeaponSlot();
        }

        protected override void Start()
        {
            LoadWeaponsOnBothHands();
        }

        private void InitializeWeaponSlot()
        {
            var weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (var slot in weaponSlots)
            {
                if (slot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = slot;
                }
                else if (slot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = slot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();
            LoadLeftWeapon();
        }

        // Right Weapon
        public void LoadRightWeapon()
        {

            if (player.playerInventoryManager.currentRightWeaponItem != null)
            {
                // Old Weapon
                rightHandSlot.UnloadWeapon();

                // New Weapon
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightWeaponItem.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightWeaponItem);
            }
        }

        public void SwitchRightWeapon()
        {
            if (!player.IsOwner) return;

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // Disable Two Handing If Two Handing
            
            // Check Weapon Index
            player.playerInventoryManager.rightHandWeaponIndex += 1;
            
            // Clamp Index
            if (player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                player.playerInventoryManager.rightHandWeaponIndex = 0;
                // Check If we are holding more than one
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = -0; i < player.playerInventoryManager.weaponInRightHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponInRightHandSlots[i].itemID !=
                        WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;

                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    player.playerNetworkManager.currentRightWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentRightWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (var weapon in player.playerInventoryManager.weaponInRightHandSlots)
            {
                // If The weapon is not the unarmed weapon
                if (player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID !=
                    WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                    // Assign the network weapon ID so it switched for connected clients
                    player.playerNetworkManager.currentRightWeaponID.Value = player.playerInventoryManager.weaponInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightWeapon();
            }

        }
        // Left Weapon

        public void LoadLeftWeapon()
        {
            if (!player.playerInventoryManager.currentLeftWeaponItem) return;

            if (player.playerInventoryManager.currentLeftWeaponItem != null)
            {
                // Old Weapon
                leftHandSlot.UnloadWeapon();

                // New Weapon
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftWeaponItem.weaponModel);
                leftHandSlot.LoadWeapon(player.playerInventoryManager.currentLeftWeaponItem.weaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftWeaponItem);
            }
        }

        public void SwitchLeftWeapon()
        {
            if (!player.IsOwner) return;

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            // Disable Two Handing If Two Handing
            
            // Check Weapon Index
            player.playerInventoryManager.leftHandWeaponIndex += 1;
            
            // Clamp Index
            if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
            {
                player.playerInventoryManager.leftHandWeaponIndex = 0;
                // Check If we are holding more than one
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = -0; i < player.playerInventoryManager.weaponInLeftHandSlots.Length; i++)
                {
                    if (player.playerInventoryManager.weaponInLeftHandSlots[i].itemID !=
                        WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;

                        if (firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponInLeftHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCount <= 1)
                {
                    player.playerInventoryManager.leftHandWeaponIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    player.playerNetworkManager.currentLeftWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
                    player.playerNetworkManager.currentLeftWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (var weapon in player.playerInventoryManager.weaponInLeftHandSlots)
            {
                // If The weapon is not the unarmed weapon
                if (player.playerInventoryManager.weaponInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID !=
                    WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = player.playerInventoryManager.weaponInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];
                    // Assign the network weapon ID so it switched for connected clients
                    player.playerNetworkManager.currentLeftWeaponID.Value = player.playerInventoryManager.weaponInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
            {
                SwitchLeftWeapon();
            }

        }

        // Damage Colliders

        public void OpenDamageCollider()
        {
            // Right Weapon
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.playerSoundFXManager.PlaySoundFX(WorldSoundFXManager.Instance.ChooseRandomSFX(player.playerInventoryManager.currentRightWeaponItem.whooshes));
            } 
            // Left Weapon
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
                player.playerSoundFXManager.PlaySoundFX(WorldSoundFXManager.Instance.ChooseRandomSFX(player.playerInventoryManager.currentLeftWeaponItem.whooshes));
            }

            // Play Whoosh SFX
        }

        public void CloseDamageCollider()
        {
            // Right Weapon
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            // Left Weapon
            else if (player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }

            // Play Whoosh SFX
        }

    }
}