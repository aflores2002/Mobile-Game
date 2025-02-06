using UnityEngine;
using SimpleInputNamespace;

public class CatKnightController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private AttackManager attackManager;

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
    private float originalGravityScale;

    void Start()
    {
        Debug.Log("CatKnight: Start initialized");
        rb = GetComponent<Rigidbody2D>();

        // Store original gravity scale
        originalGravityScale = rb.gravityScale;

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
        rb.gravityScale = originalGravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        attackManager = GetComponent<AttackManager>();
        if (attackManager == null)
        {
            Debug.LogError("AttackManager component missing!");
        }
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
            // When attacking, freeze all movement
            rb.velocity = Vector2.zero;
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
        if (canAttack)
        {
            Debug.Log("CatKnight: Starting attack");
            StartAttack(false);
        }
    }

    public void OnPowerAttackInput()
    {
        Debug.Log("CatKnight: OnPowerAttackInput called");
        if (canAttack)
        {
            Debug.Log("CatKnight: Starting power attack");
            StartAttack(true);
        }
    }

    [Header("Power Attack")]
    public float powerAttackSlowdownFactor = 0.3f;

    // Modify existing StartAttack to accept parameter
    private void StartAttack(bool isPowerAttack = false)
    {
        canAttack = false;
        isAttacking = true;

        // Reset any current animation states that might interfere
        animator.ResetTrigger(JumpHash);
        animator.ResetTrigger(AttackHash);

        // Force immediate animation update
        animator.Update(0f);
        animator.SetTrigger(AttackHash);

        // Execute the attack
        if (isPowerAttack)
        {
            attackManager.ExecutePowerAttack();
        }
        else
        {
            attackManager.ExecuteNormalAttack();
        }

        // Freeze position and disable gravity while preserving facing direction
        float currentXVelocity = rb.velocity.x;
        rb.velocity = new Vector2(currentXVelocity * 0.5f, 0f); // Maintain some horizontal momentum
        rb.gravityScale = 0f;

        // Add a safety timeout to reset attack state
        Invoke("ForceAttackReset", 1.0f);
    }

    public void OnAttackComplete()
    {
        Debug.Log("CatKnight: OnAttackComplete called");
        isAttacking = false;
        // Restore gravity
        rb.gravityScale = originalGravityScale;
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
            // Restore gravity
            rb.gravityScale = originalGravityScale;
        }
    }
}