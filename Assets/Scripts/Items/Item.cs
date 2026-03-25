using UnityEngine;

namespace CFS
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")]
        public string itemName;
        public Sprite itemIcon;
        [TextArea] public string itemDescription;
        public int itemID;

        public virtual void Fire()
        {
            Debug.LogWarning("");
        }

    }
}
