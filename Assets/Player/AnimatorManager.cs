using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    private PlayerManager playerManager;
    private PlayerLocomotion playerLocomotion;
    int horizontal, vertical;

    void Awake()
    {
        while (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();

        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool useRootMotion = false)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.SetBool("isUsingRootMotion", true);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // Animation Snapping

        float snappedHorizontal, snappedVertical;

        #region Snapping
        if (horizontalMovement > 0 && horizontalMovement <= 0.55f) { snappedHorizontal = 0.5f; }
        else if (horizontalMovement > 0.55f) { snappedHorizontal = 1f; }
        else if (horizontalMovement < 0 && horizontalMovement >= -0.55f) { snappedHorizontal = -0.5f; }
        else if (horizontalMovement < -0.55f) { snappedHorizontal = -1f; }
        else { snappedHorizontal = 0f; }

        if (verticalMovement > 0 && verticalMovement <= 0.55f) { snappedVertical = 0.5f; }
        else if (verticalMovement > 0.55f) { snappedVertical = 1f; }
        else if (verticalMovement < 0 && verticalMovement >= -0.55f) { snappedVertical = -0.5f; }
        else if (verticalMovement < -0.55f) { snappedVertical = -1f; }
        else { snappedVertical = 0f; }
        #endregion

        if (isSprinting)
        {
            snappedVertical = 2;
            snappedHorizontal = horizontalMovement;
        }

        // Animation
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        // Handle root motion here if needed
        if (playerManager.isUsingRootMotion)
        {
            playerLocomotion.playerRigidbody.linearDamping = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            if (deltaPosition != Vector3.zero)
            {
                deltaPosition.y = 0;
                Vector3 velocity = deltaPosition / Time.deltaTime;
                playerLocomotion.playerRigidbody.linearVelocity = velocity;
            }
        }
    }
}
