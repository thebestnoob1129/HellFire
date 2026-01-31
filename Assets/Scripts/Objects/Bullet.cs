using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float damage;

    public LayerMask groundLayer;
    public int penetration = 1;

    public GameObject owner;

    private void Start()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Entity>(out var entity))
        {
            Debug.Log("Damage", other);
            entity.OnDamaged(damage);
            Destroy(gameObject);
        }
    }
}
