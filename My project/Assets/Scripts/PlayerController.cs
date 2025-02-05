using UnityEngine;
using SimpleInputNamespace;

public class CatKnightController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Animation Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    // Private fields
    private Rigidbody2D rb;
    private bool canAttack = true;
    private float attackTimer;
    private float horizontalMovement;
    private bool isAttacking = false;

    void Start()
    {
        Debug.Log("CatKnight: Start initialized");
        rb = GetComponent<Rigidbody2D>();

        // Ensure we have references
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log("CatKnight: Got animator component");
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Debug.Log("CatKnight: Got sprite renderer component");
        }

        // Setup rigidbody for platformer physics
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // Get input from the horizontal joystick
        horizontalMovement = SimpleInput.GetAxis("Horizontal");

        // Handle attack cooldown
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
                Debug.Log("CatKnight: Attack cooldown reset, can attack again");
            }
        }

        // Update animations
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // Only move horizontally if not attacking
        if (!isAttacking)
        {
            // Preserve vertical velocity while changing horizontal velocity
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = horizontalMovement * moveSpeed;
            rb.velocity = newVelocity;
        }
        else
        {
            // When attacking, only stop horizontal movement
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = 0;
            rb.velocity = newVelocity;
        }
    }

    void UpdateAnimations()
    {
        // Only update movement animations if not attacking
        if (!isAttacking)
        {
            // Calculate movement speed for animation using absolute horizontal movement
            float speed = Mathf.Abs(horizontalMovement);
            animator.SetFloat(SpeedHash, speed);

            // Update facing direction based on movement
            if (horizontalMovement != 0)
            {
                spriteRenderer.flipX = horizontalMovement < 0;
            }
        }
    }

    public void OnJumpInput()
    {
        Debug.Log("CatKnight: Jump input received");
        if (!isAttacking)
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        animator.SetTrigger(JumpHash);

        // Apply jump force
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        Debug.Log($"CatKnight: Jump started with force {jumpForce}");
    }

    public void OnAttackInput()
    {
        Debug.Log("CatKnight: OnAttackInput called");
        if (canAttack && !isAttacking)
        {
            Debug.Log("CatKnight: Starting attack");
            StartAttack();
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        isAttacking = true;
        Debug.Log("CatKnight: Setting Attack trigger");
        animator.SetTrigger(AttackHash);

        // When attacking, only stop horizontal movement
        Vector2 newVelocity = rb.velocity;
        newVelocity.x = 0;
        rb.velocity = newVelocity;

        // Add a safety timeout to reset attack state
        Invoke("ForceAttackReset", 1.0f);
    }

    public void OnAttackComplete()
    {
        Debug.Log("CatKnight: OnAttackComplete called");
        isAttacking = false;
        CancelInvoke("ForceAttackReset");
    }

    private void ForceAttackReset()
    {
        if (isAttacking)
        {
            Debug.LogWarning("CatKnight: Force resetting attack state due to timeout");
            isAttacking = false;
            canAttack = true;
            attackTimer = 0;
        }
    }
}