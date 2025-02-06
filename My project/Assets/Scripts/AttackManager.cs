using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Attack Settings")]
    public int normalAttackDamage = 100;
    public int powerAttackDamage = 200;
    public float attackRange = 1.5f;
    public float attackHeight = 2.5f;
    public LayerMask enemyLayer;

    [Header("Attack Position Offset")]
    public float horizontalOffset = 0.5f;  // How far forward the attack range extends
    public float verticalOffset = 0f;      // Move attack range up or down

    [Header("Debug")]
    public bool showAttackRange = true;
    public Color attackRangeColor = Color.red;

    private void PerformAttack(int damage)
    {
        // Get the attack position based on the player's facing direction
        Vector2 attackPosition = transform.position;
        bool isFacingRight = !GetComponent<SpriteRenderer>().flipX;

        // Apply offsets (flip horizontal offset based on facing direction)
        attackPosition.x += isFacingRight ? horizontalOffset : -horizontalOffset;
        attackPosition.y += verticalOffset;

        Debug.Log($"Attack position: {attackPosition}");

        // Create a box for hit detection
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPosition,
            new Vector2(attackRange, attackHeight),
            0f,
            enemyLayer
        );

        Debug.Log($"Found {hitEnemies.Length} enemies in range");

        // Apply damage to all enemies hit
        foreach (Collider2D enemy in hitEnemies)
        {
            BaseEnemy enemyComponent = enemy.GetComponent<BaseEnemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage);
                Debug.Log($"Hit enemy for {damage} damage!");
            }
        }
    }

    public void ExecuteNormalAttack()
    {
        PerformAttack(normalAttackDamage);
    }

    public void ExecutePowerAttack()
    {
        PerformAttack(powerAttackDamage);
    }

    private void OnDrawGizmos()
    {
        if (!showAttackRange) return;

        Vector2 attackPosition = transform.position;
        bool isFacingRight = !GetComponent<SpriteRenderer>().flipX;

        // Apply offsets for visualization
        attackPosition.x += isFacingRight ? horizontalOffset : -horizontalOffset;
        attackPosition.y += verticalOffset;

        // Draw the attack range box
        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireCube(attackPosition, new Vector3(attackRange, attackHeight, 0));

        // Draw a line from player to attack center for better visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, attackPosition);
    }
}