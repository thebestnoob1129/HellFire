using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CFS
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase Instance;

        public WeaponItem unarmedWeapon;
        [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Items")]
        private List<Item> items = new List<Item>();


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

            // Add all weapons to list of items
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
            DontDestroyOnLoad(gameObject);
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}