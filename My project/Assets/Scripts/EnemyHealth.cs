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

        // animator?.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // animator?.SetTrigger("Death");

        // Disable enemy
        gameObject.SetActive(false);

        // Destroy(gameObject, deathAnimationLength);
    }
}