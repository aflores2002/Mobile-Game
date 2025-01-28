using UnityEngine;
using SimpleInputNamespace;

public class CatKnightController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Private fields
    private Rigidbody2D rb;
    private bool canAttack = true;
    private float attackTimer;
    private Vector2 movement;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int MagicHash = Animator.StringToHash("Magic");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure we have references
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup rigidbody
        rb.gravityScale = 0; // For 8-directional movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Get input from the D-pad
        movement.x = SimpleInput.GetAxis("Horizontal");
        movement.y = SimpleInput.GetAxis("Vertical");

        // Handle attack cooldown
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }

        // Update animations
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // Move the character
        rb.velocity = movement * moveSpeed;
    }

    void UpdateAnimations()
    {
        // Calculate movement speed for animation
        float speed = movement.magnitude;
        animator.SetFloat(SpeedHash, speed);

        // Update facing direction based on movement
        if (movement.x != 0)
        {
            spriteRenderer.flipX = movement.x < 0;
        }

        // For 8-directional animation, you might want to set different animation states
        // based on movement direction
        if (speed > 0)
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            // Normalize angle to 0-360 range
            if (angle < 0) angle += 360;

            // You can use this angle to determine which directional animation to play
            // Example: Divide into 8 directions (45-degree segments)
            int direction = Mathf.RoundToInt(angle / 45f) % 8;
            // Set appropriate animation parameter based on direction
            // This depends on how your animator is set up
        }
    }

    public void OnAttackInput()
    {
        if (canAttack)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        animator.SetTrigger(AttackHash);

        // Attack logic
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Deal damage to enemy
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(1);
        }
    }

    public void TakeDamage()
    {
        animator.SetTrigger(HurtHash);
        // Add damage logic here
    }

    public void CastMagic()
    {
        animator.SetTrigger(MagicHash);
        // Add magic casting logic here
    }

    public void Die()
    {
        animator.SetTrigger(DeathHash);
        // Add death logic here
    }

    // Helper method to visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}