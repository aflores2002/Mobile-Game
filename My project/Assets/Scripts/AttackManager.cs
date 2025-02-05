using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [Header("Attack Settings")]
    public int normalAttackDamage = 100;
    public float attackRange = 1.5f;
    public LayerMask enemyLayer;

    [Header("Debug")]
    public bool showAttackRange = true;

    private void PerformAttack(int damage)
    {
        Debug.Log("Performing attack...");

        // Get the attack position based on the player's facing direction
        Vector2 attackPosition = transform.position;
        bool isFacingRight = !GetComponent<SpriteRenderer>().flipX;
        attackPosition.x += isFacingRight ? attackRange / 2 : -attackRange / 2;

        Debug.Log($"Attack position: {attackPosition}");

        // Create a box for hit detection
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPosition,
            new Vector2(attackRange, 1.5f),
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

    // Call this method to perform a normal attack
    public void ExecuteNormalAttack()
    {
        PerformAttack(normalAttackDamage);
    }

    // Visualization for debug purposes
    private void OnDrawGizmos()
    {
        if (!showAttackRange) return;

        Vector2 attackPosition = transform.position;
        bool isFacingRight = !GetComponent<SpriteRenderer>().flipX;
        attackPosition.x += isFacingRight ? attackRange / 2 : -attackRange / 2;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPosition, new Vector3(attackRange, 1.5f, 0));
    }
}