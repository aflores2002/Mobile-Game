using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public bool IsAlive => currentHealth > 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation if you have one
        // animator?.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death animation if you have one
        // animator?.SetTrigger("Death");

        // Disable enemy
        gameObject.SetActive(false);

        // You might want to destroy the enemy after a delay
        // Destroy(gameObject, deathAnimationLength);
    }
}