using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public RawImage image;
    public Color selectedColor, notSelectedColor;

    private void Start()
    {
        DeSelect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void DeSelect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            var inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }
}