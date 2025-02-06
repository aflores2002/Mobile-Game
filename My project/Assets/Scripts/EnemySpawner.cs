using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WaveConfig
{
    public int numberOfLightEnemies = 3;
    public int numberOfHeavyEnemies = 2;
    public float timeBetweenSpawns = 1.5f;
    public float timeBetweenWaves = 5f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public WaveConfig[] waves;
    public float spawnDistanceFromCenter = 12f;
    public float groundLevel = -3f;
    public float flyingHeight = 2f;
    public float flyingHeightVariation = 1f;

    [Header("Prefabs")]
    public GameObject lightEnemyPrefab;
    public GameObject heavyEnemyPrefab;

    [Header("Movement Settings")]
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 3f;
    public LayerMask groundLayer;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("EnemySpawner: No waves configured!");
            return;
        }

        StartNextWave();
    }

    void StartNextWave()
    {
        if (currentWave >= waves.Length)
        {
            Debug.Log("EnemySpawner: All waves completed!");
            return;
        }

        if (!isSpawning)
        {
            StartCoroutine(SpawnWave(waves[currentWave]));
        }
    }

    IEnumerator SpawnWave(WaveConfig wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.numberOfLightEnemies; i++)
        {
            SpawnLightEnemy();
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        for (int i = 0; i < wave.numberOfHeavyEnemies; i++)
        {
            SpawnHeavyEnemy();
            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        yield return new WaitForSeconds(wave.timeBetweenWaves);

        currentWave++;
        isSpawning = false;
        StartNextWave();
    }

    void SpawnLightEnemy()
    {
        bool spawnOnRight = Random.value > 0.5f;
        float xPos = spawnOnRight ? spawnDistanceFromCenter : -spawnDistanceFromCenter;
        float yPos = flyingHeight + Random.Range(-flyingHeightVariation, flyingHeightVariation);

        Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
        GameObject enemy = Instantiate(lightEnemyPrefab, spawnPosition, Quaternion.identity);

        EnemyMovement movement = enemy.AddComponent<EnemyMovement>();
        movement.Initialize(!spawnOnRight, true, groundLayer, bounceHeight, bounceSpeed);
    }

    void SpawnHeavyEnemy()
    {
        bool spawnOnRight = Random.value > 0.5f;
        float xPos = spawnOnRight ? spawnDistanceFromCenter : -spawnDistanceFromCenter;
        float yPos = groundLevel + 1f;

        Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
        GameObject enemy = Instantiate(heavyEnemyPrefab, spawnPosition, Quaternion.identity);

        // Setup Rigidbody2D with specific values for better bouncing
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb == null) rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.mass = 1f;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BoxCollider2D collider = enemy.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = enemy.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
        }

        EnemyMovement movement = enemy.AddComponent<EnemyMovement>();
        movement.Initialize(!spawnOnRight, false, groundLayer, bounceHeight * 2f, bounceSpeed);
    }
}

public class EnemyMovement : MonoBehaviour
{
    private bool movingRight;
    private bool isFlying;
    private float moveSpeed;
    private Vector3 startPosition;
    private float bounceOffset;
    private float bounceHeight;
    private float bounceSpeed;
    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float groundCheckDistance = 0.1f;
    private float lastBounceTime;
    private float bounceCooldown = 0.5f;
    private Vector3 originalScale;

    public void Initialize(bool moveRight, bool flying, LayerMask ground, float bHeight, float bSpeed)
    {
        movingRight = moveRight;
        isFlying = flying;
        moveSpeed = isFlying ? 3f : 1.5f;
        startPosition = transform.position;
        bounceOffset = Random.Range(0f, 2f * Mathf.PI);
        groundLayer = ground;
        bounceHeight = bHeight;
        bounceSpeed = bSpeed;
        originalScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        if (!isFlying && rb != null)
        {
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (isFlying)
        {
            UpdateFlyingMovement();
        }
        else
        {
            UpdateGroundMovement();
        }

        // Update sprite direction
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.flipX = !movingRight;
        }
    }

    void UpdateFlyingMovement()
    {
        float movement = moveSpeed * (movingRight ? 1 : -1) * Time.deltaTime;
        Vector3 position = transform.position;

        // Flying enemies use sine wave motion
        float bounce = Mathf.Sin((Time.time + bounceOffset) * bounceSpeed) * bounceHeight;
        position.y = startPosition.y + bounce;
        position.x += movement;

        transform.position = position;
    }

    void UpdateGroundMovement()
    {
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found!");
            return;
        }

        // Ground check with debug visualization
        Vector2 rayStart = transform.position;
        Vector2 rayEnd = rayStart + Vector2.down * groundCheckDistance;
        isGrounded = Physics2D.Raycast(rayStart, Vector2.down, groundCheckDistance, groundLayer);

        // Visual debug of ground check
        Debug.DrawLine(rayStart, rayEnd, isGrounded ? Color.green : Color.red);

        // Move horizontally
        float movement = moveSpeed * (movingRight ? 1 : -1);
        rb.velocity = new Vector2(movement, rb.velocity.y);

        // Apply bounce and scale effect when grounded
        if (isGrounded && Time.time > lastBounceTime + bounceCooldown && rb.velocity.y <= 0.1f)
        {
            Debug.Log("Applying bounce!");
            // Use much stronger impulse force for more visible bounce
            rb.AddForce(Vector2.up * 500f, ForceMode2D.Impulse);
            lastBounceTime = Time.time;

            // Apply squash effect
            StartCoroutine(SquashAndStretch());
        }
    }

    IEnumerator SquashAndStretch()
    {
        // Squash
        transform.localScale = new Vector3(
            originalScale.x * 1.2f,
            originalScale.y * 0.8f,
            originalScale.z
        );

        yield return new WaitForSeconds(0.1f);

        // Stretch
        transform.localScale = new Vector3(
            originalScale.x * 0.8f,
            originalScale.y * 1.2f,
            originalScale.z
        );

        yield return new WaitForSeconds(0.1f);

        // Return to normal
        transform.localScale = originalScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Turn around when hitting walls
        if (!isFlying && collision.contacts[0].normal.x != 0)
        {
            movingRight = !movingRight;
        }
    }
}