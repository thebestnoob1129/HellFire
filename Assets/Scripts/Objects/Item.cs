using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header("Only Gameplay")] public Vector3 bodyOffset;
    public Quaternion bodyRotation;
    [Min(0)] public int maxCapacity = 30;
    [Min(0)] public int capacity;
    [Min(0)] public float reloadTime;
    [Min(0)] public float fireRate;

    protected Vector3 forward;

    [Header("Only UI")] public bool stackable = true;

    [Header("Both")] public ItemType type;
    public ActionType actionType;
    public Texture texture;

    // Effects Array Holder

    protected bool isFiring;
    protected bool isReloading;

    private void Start()
    {
        capacity = maxCapacity;
    }

    private void Update()
    {
        forward = transform.parent ? transform.parent.forward : transform.forward;
    }

    public virtual void Fire()
    {
        if (isFiring) return;
        if (isReloading) return;
        if (capacity <= 0) Reload();

        isFiring = true;
        capacity--;
        capacity = Mathf.Clamp(capacity, 0, maxCapacity);

        Invoke(nameof(ResetFire), fireRate / 60);
    }

    public virtual void Reload()
    {
        if (isReloading) return;
        isReloading = true;
        Invoke(nameof(ResetReload), reloadTime);
    }
    
    protected void ResetFire()
    {
        isFiring = false;
    }

    protected void ResetReload()
    {
        capacity = maxCapacity;
        isReloading = false;
    }

    protected GameObject CreateProjectile(GameObject prefab, float speed, int destroyTime = 1)
    {
        // Bullet

        var projectile = prefab ? Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        projectile.transform.SetPositionAndRotation(transform.position + forward, Quaternion.identity);
        projectile.tag = "Projectile";

        var col = projectile.GetComponent<SphereCollider>() ? projectile.GetComponent<SphereCollider>() : projectile.AddComponent<SphereCollider>();
        col.isTrigger = true;

        var rb = projectile.GetComponent<Rigidbody>() ? projectile.GetComponent<Rigidbody>() : projectile.AddComponent<Rigidbody>();
        rb.AddForce(forward * (speed * 10), ForceMode.Impulse);
        rb.useGravity = false;
        
        var bul = projectile.GetComponent<Bullet>() ? projectile.GetComponent<Bullet>() : projectile.AddComponent<Bullet>();
        bul.owner = gameObject;

        Destroy(projectile, destroyTime);
        return projectile;
    }
    protected GameObject CreateProjectile(GameObject prefab, Vector3 force, int destroyTime = 1)
    {
        // Bullet

        var projectile = prefab ? Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        projectile.transform.SetPositionAndRotation(transform.position + forward, Quaternion.identity);
        projectile.tag = "Projectile";

        var col = projectile.GetComponent<SphereCollider>() ? projectile.GetComponent<SphereCollider>() : projectile.AddComponent<SphereCollider>();
        col.isTrigger = true;

        var rb = projectile.GetComponent<Rigidbody>() ? projectile.GetComponent<Rigidbody>() : projectile.AddComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
        rb.useGravity = false;
        
        var bul = projectile.GetComponent<Bullet>() ? projectile.GetComponent<Bullet>() : projectile.AddComponent<Bullet>();
        bul.owner = gameObject;

        Destroy(projectile, destroyTime);
        return projectile;
    }

}

public enum ItemType
{
    Weapon,
    Tool,
}

public enum ActionType
{
    Damage,
    Heal,
    Repair
}