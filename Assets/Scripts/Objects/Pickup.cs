using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour, IInteractable
{
    private bool isHolding;

    [SerializeField] private float throwforce = 600f;

    [SerializeField] private float maxDistance = 3f;
    private float distance;
    private Vector3 objectPos;

    private PickUpScript tempParent;
    private Rigidbody body;

    // Create Ray Cast to detect if object is colliding with object and set postition to hit.point + transform.size/2

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        tempParent = PickUpScript.Instance;
    }

    private void FixedUpdate()
    {
        distance = Vector3.Distance(tempParent.transform.position, transform.position);
        if (isHolding) { Hold(); }
    }

    public void Interact()
    {

    }

    private void HandleHold()
    {
        if(!isHolding && distance <= maxDistance)
        {
            body.isKinematic = true;
            body.useGravity = false;
            body.detectCollisions = true;

            transform.SetParent(tempParent.holdPos);
            transform.localPosition = Vector3.zero;
            isHolding = true;
        }
        else
        {
            body.isKinematic = false;
            transform.SetParent(null);
            isHolding = false;
            body.AddForce(tempParent.player.transform.forward * throwforce);
        }
    }

    private void OnMouseUp()
    {
        Drop();
    }

    private void OnMouseExit()
    {
        Drop();
    }

    private void Hold()
    {
        if (distance >= maxDistance) Drop();
        
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    private void Throw()
    {
        body.AddForce(tempParent.transform.forward * throwforce);
        Drop();
    }

    private void Drop()
    {
        if (!isHolding) return;

        isHolding = false;
        objectPos = transform.position;
        transform.position = objectPos;
        transform.SetParent(null);
        body.useGravity = true;
    }

}
