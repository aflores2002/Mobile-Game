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
    private Vector2 movement;
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
        rb.gravityScale = 3f; // Adjust this value to control fall speed
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Verify animator parameters exist
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log($"CatKnight: Found animator parameter: {param.name} of type {param.type}");
        }
    }

    void Update()
    {
        // Get input from the D-pad (only use horizontal movement now)
        movement.x = SimpleInput.GetAxis("Horizontal");
        movement.y = 0; // Ignore vertical input since we're using gravity

        // Check if we're grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        // Optional: Debug visualization of ground check
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
            // Preserve vertical velocity (gravity) while changing horizontal velocity
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = movement.x * moveSpeed;
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
        Debug.Log("CatKnight: OnAttackInput called");
        Debug.Log($"CatKnight: canAttack={canAttack}, isAttacking={isAttacking}");

        if (canAttack && !isAttacking)
        {
            Debug.Log("CatKnight: Starting attack");
            StartAttack();
        }
        else
        {
            if (!canAttack)
                Debug.Log("CatKnight: Can't attack - on cooldown");
            if (isAttacking)
                Debug.Log("CatKnight: Can't attack - already attacking");
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        isAttacking = true;
        Debug.Log("CatKnight: Setting Attack trigger");
        animator.SetTrigger(AttackHash);
        rb.velocity = Vector2.zero;

        // Add a safety timeout to reset attack state
        Invoke("ForceAttackReset", 1.0f);
    }

    // Called by Animation Event at the end of attack animation
    public void OnAttackComplete()
    {
        Debug.Log("CatKnight: OnAttackComplete called");
        isAttacking = false;
        CancelInvoke("ForceAttackReset"); // Cancel the safety timeout since we completed normally
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