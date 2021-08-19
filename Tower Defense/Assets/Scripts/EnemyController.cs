using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : FactoryObject
{
    public event EventHandler<OnEnemyDespawnEventArgs> OnEnemyDespawn;
    public class OnEnemyDespawnEventArgs : EventArgs
    {
        public bool isDead;
        public int value;
        public int damage;
    }

    public static List<EnemyController> enemyList = new List<EnemyController>();

    public static EnemyController GetClosestEnemy(Vector3 position, float maxRange)
    {
        EnemyController closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (EnemyController enemy in enemyList)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);

            if (distance <= maxRange)
            {
                if (!enemy.gameObject.activeInHierarchy || enemy.IsDead())
                {
                    continue;
                }
                else if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }

    [SerializeField] private EnemyProperties enemyProperties;

    private SpriteRenderer spriteRenderer;
    private float moveSpeed;
    private int valueOnDeath;
    private int maxHealth;

    private HealthSystem healthSystem;
    

    private Vector3[] globalWaypoints;
    private int currentWaypoint;
    private float percentBetweenWaypoints;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthSystem = new HealthSystem();
    }

    private void OnEnable()
    {
        currentWaypoint = 0;
        percentBetweenWaypoints = 0;
    }

    public void Init(EnemyProperties enemyProperties, Vector3[] waypoints)
    {
        spriteRenderer.sprite = enemyProperties.enemySprite;
        moveSpeed = enemyProperties.moveSpeed;
        valueOnDeath = enemyProperties.valueOnDeath;
        maxHealth = enemyProperties.maxHealth;

        healthSystem.SetMaxHealth(maxHealth, true);

        globalWaypoints = waypoints;
    }

    private void Update()
    {
        Vector3 velocity = CalculateWaypointMovement();
        transform.Translate(velocity);
    }

    private Vector3 CalculateWaypointMovement()
    {
        // Invalid number of waypoints
        if (globalWaypoints.Length <= 0)
        {
            Reclaim();
            return Vector3.zero;
        }

        // Reached final waypoint
        if (currentWaypoint >= globalWaypoints.Length - 1)
        {
            Reclaim();
            return Vector3.zero;
        }

        int toWaypoint = currentWaypoint + 1;

        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[currentWaypoint], globalWaypoints[toWaypoint]);
        percentBetweenWaypoints += Time.deltaTime * moveSpeed / distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[currentWaypoint], globalWaypoints[toWaypoint], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            currentWaypoint++;
        }

        return (newPos - transform.position);
    }

    public void Damage(int amount)
    {
        healthSystem.Damage(amount);

        if (IsDead())
            Reclaim();
    }

    public Vector3 GetEnemyPosition()
    {
        return transform.position;
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public override void Reclaim()
    {
        OnEnemyDespawn?.Invoke(this, new OnEnemyDespawnEventArgs { isDead = IsDead(), value = valueOnDeath, damage = maxHealth });
        OnEnemyDespawn = null;
        enemyList.Remove(this);

        base.Reclaim();
    }
}
