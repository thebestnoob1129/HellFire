using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Entity : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent navAgent;
    protected Animator animator;
    public GameObject hitPrefab;
    public float distance;
    public float limitWalk = 2f;

    [Header("Attack")]
    public Transform target;
    public GameObject weapon;
    public float attackDistance = 2.5f;
    public float timeBetweenAttacks = 1f;
    private bool hasAttacked;

    [Header("Stats")]
    public EntityType entityType;
    public float Health { get; private set; }
    [SerializeField, Min(1)] private int maxHealth = 100;
    public float speed = 5f;
    public int baseDamage = 10;
    public bool canJumpScare;

    [Header("Search")]
    public LayerMask playerLayer, groundLayer;
    public float walkPointRange = 20;
    private Vector3 walkPoint;
    private bool walkPointSet;

    // States
    public float sightRange = 5, attackRange = 10;
    public bool playerInSightRange, playerInAttackRange;
    private Ray targetRay;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Health = maxHealth;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        walkPointRange = walkPointRange < 30 ? 30 : walkPointRange;
        sightRange = sightRange < 5 ? 5 : sightRange;
        attackRange = attackRange < 5 ? 5 : attackRange;

        if (!weapon) Debug.LogWarning("No weapon assigned to " + gameObject.name, gameObject);
        else
        {
            weapon = Instantiate(weapon, transform);
            // Get Weapon Handle and put on person hand
            var hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            weapon.transform.position = hand.position;
        }

    }

    private void Update()
    {
        distance = Vector3.Distance(navAgent.transform.position, navAgent.destination);
        navAgent.speed = speed;

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        /*
        targetRay = new Ray(transform.position + transform.up, target.position);
        if (Physics.Raycast(targetRay, out var hitInfo, sightRange))
        {
            playerInSightRange = hitInfo.transform.CompareTag("Player");
        }

        if (Physics.Raycast(targetRay, out var hitInfoAttack, attackRange))
        {
            playerInAttackRange = hitInfoAttack.transform.CompareTag("Player");
        }
        */

        //playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        playerInSightRange = IsLineOfSight() && IsFront();
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);




        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

        animator.SetBool("Running", navAgent.velocity.magnitude != 0f);

        if (Health <= maxHealth) Health = maxHealth;
        if (Health <= 0)
        {
            Destroy(gameObject, 1f);
        }

    }

    #region Search
    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (distance < 1) SearchWalkPoint();

        if (walkPointSet) navAgent.SetDestination(walkPoint);


        if ((transform.position - walkPoint).magnitude < 1f)
        {
            walkPointSet = false;
        }

    }

    private void SearchWalkPoint()
    {
        var randomX = Random.Range(-walkPointRange, walkPointRange);
        var randomY = Random.Range(-walkPointRange, walkPointRange);
        var randomZ = Random.Range(-walkPointRange, walkPointRange);
        var newPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);

        walkPointSet = Physics.CheckSphere(walkPoint, 1f, groundLayer) && IsPathWalkable(newPoint);
        walkPoint = newPoint;
    }

    private void ChasePlayer()
    {
        navAgent.SetDestination(target.position);
        walkPointSet = false;
    }
    protected bool IsPathWalkable(Vector3 value)
    {
        var path = new NavMeshPath();
        navAgent.CalculatePath(value, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }
    private void AttackPlayer()
    {
        navAgent.SetDestination(transform.position);
        transform.LookAt(target);

        if (hasAttacked) return;

        hasAttacked = true;
        Invoke(nameof(Attack), timeBetweenAttacks);
    }

    private bool IsFront()
    {
        var directionOfPlayer = transform.position - target.position;
        var angle = Vector3.Angle(transform.forward, directionOfPlayer);

        if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
        {
            Debug.DrawLine(transform.position, target.position, Color.red);
            return true;
        }

        return false;
    }

    private bool IsLineOfSight()
    {
        var directionOfPlayer = target.position - transform.position;
        if (Physics.Raycast(transform.position + Vector3.up, directionOfPlayer, out var hit, sightRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Attack
    protected virtual void Attack()
    {
        Debug.Log("Attack Player!", gameObject);

        // Rotation
        var dir = target.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Attack
        if (weapon) weapon.GetComponent<Item>().Fire();
        hasAttacked = false;
    }
    
    // Stat System
    public void OnDamaged(float value)
    {
        Health -= value;

        var marker = Instantiate(hitPrefab, transform, true);
        marker.transform.localPosition = new Vector3(0, 2, 0);
        marker.GetComponent<TMP_Text>().color = Color.Lerp(Color.red, Color.darkRed, value / 1000);
        marker.GetComponent<TMP_Text>().text = Mathf.RoundToInt(value).ToString();
        marker.transform.LookAt(target);
        Destroy(marker, 1f);
    }

    #endregion    
    private void OnCollisionEnter(Collision other)
    {
        var obj = other.collider.gameObject;

        if (obj.GetComponent<Bullet>()) OnDamaged(obj.GetComponent<Bullet>().damage);
        if (obj.TryGetComponent<PlayerStats>(out var player)) player.OnDamaged(baseDamage);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.darkTurquoise;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(walkPoint, 1f);


    }

    private void OnAnimatorMove()
    {
        if (animator.GetBool("Running"))
        {
            navAgent.speed = (animator.deltaPosition / Time.deltaTime).magnitude + speed;
        }
    }

    public enum EntityType
    {
        Range,
        Melee,
        JumpScare
    }
}