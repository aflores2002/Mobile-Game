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
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        onDamageTaken?.Invoke();

        // Visual feedback
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRoutine()
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