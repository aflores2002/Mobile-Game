using UnityEngine;

public class HeavyEnemy : BaseEnemy
{
    [Header("Heavy Enemy Settings")]
    public float moveSpeed = 1.5f;
    private bool movingRight = true;
    private Rigidbody2D rb;

    void Awake()
    {
        maxHealth = 400;  // Set maxHealth in Awake, before Start is called
        Debug.Log($"HeavyEnemy Awake - Set maxHealth to: {maxHealth}");
    }

    protected override void Start()
    {
        Debug.Log($"HeavyEnemy Start - Before base.Start(), maxHealth={maxHealth}");
        base.Start();
        Debug.Log($"HeavyEnemy Start - After base.Start(), currentHealth={currentHealth}, maxHealth={maxHealth}");

        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 3f;
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Ensure we have a collider
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
        }
    }

    void Update()
    {
        if (rb == null) return;

        // Move horizontally
        float movement = moveSpeed * (movingRight ? 1 : -1);
        rb.velocity = new Vector2(movement, rb.velocity.y);

        // Update sprite direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !movingRight;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Turn around when hitting walls
        if (collision.contacts[0].normal.x != 0)
        {
            movingRight = !movingRight;
        }
    }

    public override void TakeDamage(int damage)
    {
        Debug.Log($"HeavyEnemy TakeDamage - Before base.TakeDamage: currentHealth={currentHealth}, maxHealth={maxHealth}");
        base.TakeDamage(damage);
        Debug.Log($"HeavyEnemy TakeDamage - After base.TakeDamage: currentHealth={currentHealth}, maxHealth={maxHealth}");
    }

    protected override void Die()
    {
        Debug.Log($"HeavyEnemy Die - currentHealth={currentHealth}");
        if (currentHealth <= 0)
        {
            base.Die();
        }
    }
}