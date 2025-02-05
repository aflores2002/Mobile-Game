using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEnemy : BaseEnemy
{
    [Header("Heavy Enemy Settings")]
    public float moveSpeed = 1.5f;

    protected override void Start()
    {
        maxHealth = 200; // Heavy enemies have more health
        base.Start();
    }

    protected override void Die()
    {
        // Add any heavy enemy specific death behavior here
        base.Die();
    }
}
