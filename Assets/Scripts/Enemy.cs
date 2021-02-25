using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //UI elements
    public Slider enemyBarFill;
    public GameObject enemyBarUI;

    public Animator animator;               // The animator object assigned to the script. (This case should be the enemy)
    public LayerMask groundLayer;           // Layer that includes the ground.

    [Header("Health Setting")]
    // Health variables
    private int maxEnemyHealth = 150;        // Maximum possible enemy health
    private HealthClass enemyHealth;        // An Object of the health class which will hold the health information of the enemy.

    [Header("Attack Variables")]
    // Attack variables
    public Transform enemyAttackPoint;      // The point at which the attack is released from the enemy. This is usally at the Axe
    public float enemyAttackRange = 0.5f;   // Distance at which the enemy attack is delivered
    public LayerMask playerLayer;           // Layer that contains the player GameObject
    public int enemyAttackDamage = 32;      // Damage the enemy can give in an attack
    public float enemyAttackRate = 2f;      // Rate of attack the enemy has
    private float nextEnemyAttackTime = 0f;
    public bool playerInAttackRange;

    float walkSpeed = 2f;
    float runSpeed = 4f;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float enemySightRadius;
    public bool playerInSight;

    [Header("Agents")]
    // NavMeshAgent variables
    public NavMeshAgent enemyNavMeshAgent;
    private Transform player;


    
    public float EnemySpeed 
    {
        get { return enemyNavMeshAgent.speed; }
        private set { enemyNavMeshAgent.speed = value; } 
    }

    // Start is called before the first frame update
    void Start()
    {
        // The game starts with the enemy health as it's max health.
        // The Health class is responsible for this
        enemyHealth = new HealthClass(maxEnemyHealth);

        // Set the fill value of healthbar as its current health
        UpdateHealthBar();

        player = GameObject.Find("Player").transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Updates the health bar with the current health of the enemy.
    void UpdateHealthBar()
    {
        enemyBarFill.value = enemyHealth.CalculateCurrentHealth();
    }

    // Update is called once per frame
    void Update()
    {
        //Activate healthbar UI
        UpdateHealthBar();

        // Check if player is in sight.
        playerInSight = Physics.CheckSphere(transform.position, enemySightRadius, playerLayer);
        playerInAttackRange = Physics.CheckSphere(enemyAttackPoint.position, enemyAttackRange, playerLayer);

        // If player is not in sight and not in attack range.
        if (!playerInSight && !playerInAttackRange)
        {
            Patrol();
        }

        // If player is in sight, then follow
        if (playerInSight && !playerInAttackRange)
        {
            animator.SetBool("IsRunning", true);
            FollowPlayer();
        }

        if (playerInAttackRange == true && Time.time >= nextEnemyAttackTime)
        {
            Attack();
            // Do not attack immediately. Attack after 1 seconds. 
            nextEnemyAttackTime = Time.time + 1.5f / enemyAttackRate;
        }
    }

    /// <summary>
    /// Function that makes the agent patrol/wander around the map.
    /// </summary>
    void Patrol()
    {
        // Play walking animation
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsRunning", false);

        //Set the enemy speed
        EnemySpeed = walkSpeed;

        // If no walk point was set, find one.
        if (!walkPointSet)
            SetWalkPoint();

        // If walkpoint was set, move nav mesh to the point
        if (walkPointSet)
            enemyNavMeshAgent.SetDestination(walkPoint);

        // Calculate distance to walkpoint by subtracting position and the walk point
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // If the point was reached, then set point to false.
        if (distanceToWalkPoint.magnitude < 2f)
            walkPointSet = false;
    }

    /// <summary>
    /// Function to get a random point to walk to.
    /// </summary>
    private void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Turn on flag to set walkpoint to true
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    /// <summary>
    /// Sets the NavMeshAgent's destination to go to.
    /// </summary>
    void FollowPlayer()
    {
        enemyNavMeshAgent.destination = player.position;
        EnemySpeed = runSpeed;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", true);
    }

    /// <summary>
    /// Called when the enemy attacks
    /// </summary>
    void Attack()
    {
        animator.SetTrigger("Attack");
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", true);
        FindObjectOfType<PlayerController>().PlayerTakesDamage(enemyAttackDamage);
    }

    /// <summary>
    /// Called when the enemy is attacked! It takes in the damage given to the enemy from the caller.
    /// </summary>
    public void EnemyTakesDamage(int damage)
    {
        // Reduce the enemy health by the damage factor
        enemyHealth.ReduceHealth(damage);

        //Plays a hurt animation
        animator.SetTrigger("GetHit");

        // Check if the enemy health reaches 0 or below
        if (enemyHealth.CurrentHealth <= 0)
        {
            // Call die function.
            EnemyDies();
        }
    }

    /// <summary>
    /// Called when the enemy dies.
    /// It sets the death animation of the animator object and disables the enemy script (using "this").
    /// </summary>
    void EnemyDies()
    {
        // Play die animation
        animator.SetBool("IsDead", true);
        enemyBarUI.SetActive(false);

        // Disable enemy script to disable all further actions from it.
        GetComponent<Collider>().enabled = false; // Disables the enemy collider so that the player cannot still attack the enemy
        this.enabled = false; // Disables the full script
        Destroy(gameObject, 2.8f);

    }

    /// <summary>
    /// Draw a sphere around the attackpoint
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (enemyAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
        Gizmos.DrawWireSphere(transform.position, enemySightRadius);
    }
}
