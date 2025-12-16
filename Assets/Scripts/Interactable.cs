using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    
    private Rigidbody _rigidbody;

    [SerializeField] private bool isInteractable = false;
    [SerializeField] private bool isHoldable;

    private void Awake()
    {
        while (_rigidbody == null)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    private void Update()
    {
        
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && isInteractable)
        {
            //isInteractedWith = true;
            if (isHoldable)
            {
                _rigidbody.isKinematic = true;
            }
        }
        if (context.performed && isInteractable)
        {
            // Additional interaction logic can be added here
        }

        if (context.canceled)
        {
            OnRelease();
        }

    }

    public void OnRelease()
    {
        //isInteractedWith = false;
        if (isHoldable)
        {
            _rigidbody.isKinematic = true;
        }
    }

}
