using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Utils;

public class ProjectileController : FactoryObject
{
    [SerializeField] private float moveSpeed = 20f;
    private float distanceToDestroy = 0.1f;

    private EnemyController targetEnemy;
    private int damageAmount;

    private void Awake()
    {
        
    }

    public void Init(Vector3 startPosition, Quaternion startRotation, EnemyController target, int damage)
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        targetEnemy = target;
        damageAmount = damage;
    }

    private void Update()
    {
        if (targetEnemy == null || !targetEnemy.gameObject.activeInHierarchy || targetEnemy.IsDead())
        {
            Reclaim();
            return;
        }

        Vector3 direction = (targetEnemy.GetEnemyPosition() - transform.position).normalized;

        MoveProjectile(direction);
        SetProjectileAngle(direction);

        if (Vector3.Distance(targetEnemy.GetEnemyPosition(), transform.position) < distanceToDestroy)
        {
            targetEnemy.Damage(damageAmount);
            Reclaim();
        }
    }

    private void MoveProjectile(Vector3 dir)
    {
        Vector3 moveDelta =  dir * moveSpeed * Time.deltaTime;
        transform.position += moveDelta;
    }

    private void SetProjectileAngle(Vector3 dir)
    {
        float angle = UtilsClass.GetAngleFromVectorFloat(dir);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
