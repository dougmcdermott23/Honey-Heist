using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyProperties : ScriptableObject
{
    public Sprite enemySprite;
    [Min(1f)] public float moveSpeed = 5f;
    [Min(0)] public int valueOnDeath = 20;
    [Min(1)] public int maxHealth = 1;
}
