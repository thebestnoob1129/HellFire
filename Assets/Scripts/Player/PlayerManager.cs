using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerLocomotion))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    Animator animator;

    public GameObject mainCamera, cineCamera;
    public bool isInteracting, isUsingRootMotion;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();

        mainCamera = Camera.main.gameObject;
        cineCamera = FindFirstObjectByType<CinemachineCamera>().gameObject;
        
        if (mainCamera) mainCamera.transform.SetParent(null, true);
        if (cineCamera) cineCamera.transform.SetParent(null, true);
    }

    void Update()
    {
        inputManager.HandleAllInputs();
    }

    void FixedUpdate()
    {
        // Physics-related updates can be handled here if needed
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}