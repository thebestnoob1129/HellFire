using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name, gameObject);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
