using UnityEngine;
using UnityEngine.Events;

public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    protected int currentHealth;

    [Header("Visual Feedback")]
    public float hitFlashDuration = 0.1f;
    protected SpriteRenderer spriteRenderer;
    protected Color originalColor;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDamageTaken;

    public bool IsAlive => currentHealth > 0;

    protected virtual void Start()
    {
        Debug.Log($"BaseEnemy Start - Initial maxHealth: {maxHealth}");
        currentHealth = maxHealth;
        Debug.Log($"BaseEnemy Start - Set currentHealth to: {currentHealth}");

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        Debug.Log($"BaseEnemy TakeDamage - Before damage: maxHealth={maxHealth}, currentHealth={currentHealth}, damage={damage}");

        if (!IsAlive)
        {
            Debug.Log("BaseEnemy TakeDamage - Enemy not alive, returning");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"BaseEnemy TakeDamage - After damage: currentHealth={currentHealth}");

        onDamageTaken?.Invoke();

        // Play hit sound
        AudioManager.Instance.PlayEnemyHitSound();

        // Visual feedback
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Debug.Log("BaseEnemy TakeDamage - Health <= 0, calling Die()");
            Die();
        }
    }

    protected System.Collections.IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    protected virtual void Die()
    {
        onDeath?.Invoke();
        gameObject.SetActive(false);
    }

    // Helper method to get health percentage (useful for UI)
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}