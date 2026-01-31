using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager: MonoBehaviour
{
    public static InventoryManager Instance;

    private GameObject player;
    private InputManager inputManager;
    public GameObject mainObject, shaderObject;

    [Header("Items")]
    public Item[] startItems;
    public readonly int maxStackedItems = 256;
    private int selectedSlot = -1;
    
    [Header("Slots")]
    public InventorySlot[] inventorySlots;
    private InputAction nextAction, prevAction, invAction;

    private void Awake()
    {
        Instance = this;
        inputManager = FindFirstObjectByType<InputManager>();
        player = GameObject.FindWithTag("Player");
        invAction = inputManager.invAction;
        nextAction = inputManager.nextAction;
        prevAction = inputManager.prevAction;
    }

    private void Start()
    {
        ChangeSelectedSlot(0);
        foreach (var item in startItems)
        {
            AddItem(item);
        }

        Cursor.lockState = CursorLockMode.Confined;

    }

    private void Update()
    {

        if (invAction.triggered)
        {
            mainObject.SetActive(!mainObject.activeSelf);
            shaderObject.SetActive(!shaderObject.activeSelf);

            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.Confined
                : CursorLockMode.Locked;
        }

        if (nextAction.triggered)
        {
            var val = selectedSlot + 1;
            ChangeSelectedSlot(val);
        }

        if (prevAction.triggered)
        {
            var val = selectedSlot - 1;
            ChangeSelectedSlot(val);
        }

    }

    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].DeSelect();
        }

        selectedSlot = Mathf.Clamp(newValue, 0, 4);
        inventorySlots[selectedSlot].Select();
    }

    public bool AddItem(Item item)
    {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (!itemInSlot) break;

            if (itemInSlot &&
                itemInSlot.item == item &&
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable)
            {
                Debug.Log("Add Slot Count", slot);
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }

        }

        // Find Empty Slot
        for (var i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>().item;
            if (!itemInSlot)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        Debug.LogWarning("None Available");
        return false;
    }

    public bool RemoveItem(Item item)
    {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (!itemInSlot) break;

            if (itemInSlot &&
                itemInSlot.item == item)
            {
                Debug.Log("Remove Slot Count", slot);
                itemInSlot.count--;
                itemInSlot.RefreshCount();

                if (itemInSlot.count > 0) return false;

                item.gameObject.SetActive(true);

                // Remove Item From Player
                item.transform.SetParent(null, true);
                item.GetComponent<BoxCollider>().isTrigger = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                itemInSlot.item = null;
                return true;
            }

        }

        return false;
    }


    private void SpawnNewItem(Item item, InventorySlot slot)
    {
        var obj = Instantiate(item.gameObject, item.bodyOffset, item.bodyRotation, player.GetComponent<PlayerManager>().mainCamera.transform);
        obj.name = item.name;
        obj.SetActive(false);
        obj.GetComponent<BoxCollider>().isTrigger = true;
        obj.GetComponent<Rigidbody>().useGravity = false;

        var inventoryItem = slot.GetComponentInChildren<InventoryItem>();
        inventoryItem.InitialiseItem(obj.GetComponent<Item>());

        Destroy(item.gameObject);
    }

    public Item GetSelectedItem(bool use)
    {
        var slot = inventorySlots[selectedSlot];
        var itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null && slot != null)
        {
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0) Destroy(itemInSlot.gameObject);
                else{ itemInSlot.RefreshCount(); }
            }
        }

        return itemInSlot.item ? itemInSlot.item : null;
    }

}