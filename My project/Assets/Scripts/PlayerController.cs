using UnityEngine;
using SimpleInputNamespace;

public class CatKnightController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Animation Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    // Private fields
    private Rigidbody2D rb;
    private bool canAttack = true;
    private float attackTimer;
    private Vector2 movement;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure we have references
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup rigidbody
        rb.gravityScale = 0;
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
        // Only move if not attacking
        if (!isAttacking)
        {
            rb.velocity = movement * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop movement during attack
        }
    }

    void UpdateAnimations()
    {
        // Only update movement animations if not attacking
        if (!isAttacking)
        {
            // Calculate movement speed for animation
            float speed = movement.magnitude;
            animator.SetFloat(SpeedHash, speed);

            // Update facing direction based on movement
            if (movement.x != 0)
            {
                spriteRenderer.flipX = movement.x < 0;
            }
        }
    }

    public void OnAttackInput()
    {
        if (canAttack && !isAttacking)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        isAttacking = true;
        animator.SetTrigger(AttackHash);

        // Stop movement during attack
        rb.velocity = Vector2.zero;
    }

    // Called by Animation Event at the end of attack animation
    public void OnAttackComplete()
    {
        isAttacking = false;
    }
}