using UnityEngine;

public class HidingSpot : MonoBehaviour, IInteractable
{
    public Vector3 releasePositon;
    public Vector3 holdPositon;
    public void Interact()
    {
        Debug.Log("Interacted with: " + gameObject.name, gameObject);

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + holdPositon, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + releasePositon, 0.5f);


    }
}
