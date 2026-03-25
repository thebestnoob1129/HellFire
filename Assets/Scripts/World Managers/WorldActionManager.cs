using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CFS
{
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager Instance;

        [Header("Weapon Item Actions")]
        public WeaponItemAction[] weaponItemActions;

        private void Awake()
        {
            if (!Instance)
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
            for (int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionID = i;
            }
        }

        public WeaponItemAction GetWeaponItemAction(int ID)
        {
            return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
        }

    }
}