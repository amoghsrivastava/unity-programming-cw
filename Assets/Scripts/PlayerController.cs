using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;               // The animator object assigned to the script. (This case should be the player)

    [Header("Movement Variables")]
    float walkSpeed = 3f;                       // Speed of the player
    float angularSpeed = 150f;              // Angular (rotational) speed of the player
    float angle;                            // Angle of rotation
    float gravity = 9.8f;                   // Gravity it feels
    float runSpeed = 4.2f;
    Vector3 moveDirection = Vector3.zero;   // A vector pointing to the direction the player is moving
    
    [Header("Health Setting")]
    // Health variables
    private int maxPlayerHealth = 300;            // Maximum possible player health
    private HealthClass playerHealth;

    [Header("Attack Variables")]
    // Attack variables
    public Transform playerAttackPoint;     // The point at which the attack is released from the player. This is usally at the sword.
    public float playerAttackRange = 1f;    // Distance at which the attack from the player is given
    public LayerMask enemyLayer;            // Layer that contains the enemy GameObjects
    public int playerAttackDamage = 50;     // Damage the player can give in an attack
    public float playerAttackRate = 1f;     // Rate of attack the player has
    float nextAttackTime = 0f;

    [Header("UI Variables")]
    //UI elements
    public Slider playerBarFill;
    public GameObject playerBarUI;
    public Color32 maxHealthColor = new Color32(119, 192, 0, 255);
    public Color32 lowHealthColor = new Color32(255, 50, 50, 255);
    public Image fill;
    public Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        // The game starts with the player health as it's max health.
        playerHealth = new HealthClass(maxPlayerHealth);
        // Set the fill value of healthbar as its current health
        UpdateHealthDetails();
        // Referencing
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Updates the health information with the current health of the player.
    void UpdateHealthDetails()
    {
        healthText.text = "❤ " + playerHealth.CurrentHealth.ToString();
        playerBarFill.value = playerHealth.CalculateCurrentHealth();
        fill.color = Color.Lerp(lowHealthColor, maxHealthColor, playerHealth.CalculateCurrentHealth());
    }

    // Update is called once per frame
    void Update()
    {
        //Activate healthbar UI
        UpdateHealthDetails();

        Movement();
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown("space"))
            {
                Attack();
                nextAttackTime = Time.time + 0.3f / playerAttackRate;
            }
        }
        
    }

    /// <summary>
    /// Function responsible for movement features of the player. It checks for presses of w or horizontal keys.
    /// </summary>
    void Movement()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey("w"))
            {
                moveDirection = transform.TransformDirection(new Vector3(0, 0, 1) * walkSpeed);
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsIdle", false);
            }
            if (Input.GetKey("w") && Input.GetKey("left shift"))
            {
                moveDirection = transform.TransformDirection(new Vector3(0, 0, 1) * runSpeed);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsIdle", false);
            }
            if (Input.GetKeyUp("w") || Input.GetKeyUp("left shift"))
            {
                moveDirection = Vector3.zero;
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsIdle", true);
            }

        }

        angle += Input.GetAxis("Horizontal") * angularSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, angle, 0);

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// This function does three things,
    /// 1. Play an attack animation
    /// 2. Detect enemies in range of attack motion
    /// 3. Apply damage to enemies
    /// </summary>
    void Attack()
    {
        // Performs attack based on our parameter "Attack"
        animator.SetTrigger("Attack");

        Collider[] hitEnemies = Physics.OverlapSphere(playerAttackPoint.position, playerAttackRange, enemyLayer);
        // To Detect enemies in range of attack
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().EnemyTakesDamage(playerAttackDamage);
        }
    }

    /// <summary>
    /// Called when the player is attacked! It takes in the damage given to the player from the caller.
    /// </summary>
    public void PlayerTakesDamage(int damage)
    {
        // Reduce the enemy health by the damage factor
        playerHealth.ReduceHealth(damage);

        //Plays a hurt animation
        animator.SetTrigger("GetHit");

        // Check if the enemy health reaches 0 or below
        if (playerHealth.CurrentHealth <= 0)
        {
            // Call die function.
            PlayerDies();
        }
    }

    /// <summary>
    /// Called when the player dies.
    /// It sets the death animation of the animator object and disables the player script (using "this").
    /// </summary>
    void PlayerDies()
    {
        // Play die animation
        animator.SetBool("IsDead", true);

        // Disable player script to disable all further actions from it.
        GetComponent<Collider>().enabled = false; // Disables the player collider so that the enemy cannot still attack the player
        this.enabled = false; // Disables the full script
    }

    /// <summary>
    /// Draw a sphere around the attackpoint
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (playerAttackPoint == null)
            return;
        Gizmos.DrawWireSphere(playerAttackPoint.position, playerAttackRange);
    }
}
