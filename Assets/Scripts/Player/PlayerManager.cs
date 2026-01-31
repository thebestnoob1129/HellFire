using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerLocomotion))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerManager : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private InputManager inputManager;
    public bool isFiring, isReloading;

    [Header("Items")]
    [SerializeField] private PlayerLocomotion playerLocomotion;
    public Item[] itemList;
    public Item currentItem;

    [Header("Camera")]
    [SerializeField] private Animator animator;
    public GameObject mainCamera, cineCamera;
    public bool isInteracting, isUsingRootMotion;

    [Header("Pickup")]
    public GameObject holdObject;
    public Transform holdPos;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();

        mainCamera = Camera.main.gameObject;
        cineCamera = FindFirstObjectByType<CinemachineCamera>().gameObject;

        if (mainCamera) mainCamera.transform.SetParent(null, true);
        if (cineCamera) cineCamera.transform.SetParent(null, true);
    }

    private void OnEnable()
    {
        var spawn = GameObject.FindGameObjectWithTag("Respawn");
        transform.position = spawn.transform.position;
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        currentItem = InventoryManager.Instance.GetSelectedItem(false);
    }

    private void FixedUpdate()
    {
        // Physics-related updates can be handled here if needed
        playerLocomotion.HandleAllMovement();
        HandleItem();
    }

    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }

    #region Items
    private void HandleItem()
    {

        foreach (var slot in InventoryManager.Instance.inventorySlots)
        {
            var item = slot.GetComponentInChildren<InventoryItem>().item;

            if (!item) break;
            item.gameObject.SetActive(item == currentItem);
        }

        if (!currentItem) return;

        currentItem.gameObject.SetActive(true);
        currentItem.transform.SetLocalPositionAndRotation(currentItem.bodyOffset, currentItem.bodyRotation);

        if (inputManager.attackInput) currentItem.Fire();
        if (inputManager.reloadInput) currentItem.Reload();
    }
    
    public void HandleDrop()
    {
        if (!currentItem) return;
        if (!InventoryManager.Instance.RemoveItem(currentItem)) return;
        currentItem = null;
    }

    public void AddItem(Item item)
    {
        InventoryManager.Instance.AddItem(item);
    }

    #region Pickup
    public void Pickup(GameObject pickUpObject)
    {
        holdObject = pickUpObject;
        var cam = mainCamera.GetComponentInChildren<Camera>().gameObject;
        pickUpObject.transform.SetParent(cam.transform);
    }


    #endregion

    #endregion

}