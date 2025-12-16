using UnityEngine;

public enum SlotTag
{
    None,
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Weapon,
    Shield,
    Accessory
}

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public SlotTag  itemTag;

    [Header("If the item can be equipped")]
    public GameObject equipmentPrefab;
}
