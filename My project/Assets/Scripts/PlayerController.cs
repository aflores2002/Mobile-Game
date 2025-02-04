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

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    private bool isGrounded;

    // Animation Parameters
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

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

        // Verify animator parameters
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log($"CatKnight: Found animator parameter: {param.name} of type {param.type}");
        }
    }

    void Update()
    {
        // Get input from the horizontal joystick
        horizontalMovement = SimpleInput.GetAxis("Horizontal");

        // Check if we're grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        // Debug visualization of ground check
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, isGrounded ? Color.green : Color.red);

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