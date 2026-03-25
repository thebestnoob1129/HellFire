using UnityEngine;

namespace CFS
{
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        // What slot is this?
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;

        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.rotation = Quaternion.identity;
            weaponModel.transform.localScale = Vector3.one;
        }
    }
}