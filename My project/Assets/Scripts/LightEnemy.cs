using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnemy : BaseEnemy
{
    [Header("Light Enemy Settings")]
    public float moveSpeed = 3f;

    protected override void Start()
    {
        maxHealth = 100; // Light enemies have less health
        base.Start();
    }

    void Awake()
    {
        // Add trigger collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }

    protected override void Die()
    {
        // Add any light enemy specific death behavior here
        base.Die();
    }
}
