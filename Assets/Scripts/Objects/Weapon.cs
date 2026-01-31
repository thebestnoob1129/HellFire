using UnityEngine;

public class Weapon : Item
{
    [Header("Weapon")] 
    public string weaponName;
    public float range = 100f;
    public int baseDamage = 10;

    [Header("Bullet")]
    public GameObject bullet;
    public float bulletSpeed = 20f;
    public int impactForce = 30;
    private bool bulletFiring;
    [SerializeField] private float bulletTime = 1f;

    public ParticleSystem muzzleFlash;

    private void Start()
    {
        type = ItemType.Weapon;
        stackable = false;
        bulletTime = bulletTime > fireRate ? fireRate : bulletTime;
        name = weaponName ?? name;
    }

    public override void Fire()
    {
        if (isFiring) return;
        if (isReloading) return;
        if (capacity <= 0) Reload();

        isFiring = true;
        capacity--;
        capacity = Mathf.Clamp(capacity, 0, maxCapacity);

        Invoke(nameof(ResetFire), fireRate / 60);

        if (bulletFiring) return;

        //var bul = CreateProjectile(bullet, bulletSpeed);
        var bul = CreateProjectile(bullet, 10f * bulletSpeed * transform.parent.forward);
        bul.GetComponent<Bullet>().damage = baseDamage;

        Invoke(nameof(ResetBullet), bulletTime / 60);
    }

    private void ResetBullet()
    {
        bulletFiring = false;
    }

    private void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play();

        if (Physics.Raycast(transform.parent.position, forward, out var hit , range))
        {
            if (hit.transform.TryGetComponent<Entity>(out var entity))
            {
                Debug.Log("Damaged");
                entity.OnDamaged(baseDamage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position + forward, forward * range);
    }
}
