using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyWave : ScriptableObject
{
    public EnemyProperties enemyType;
    public int numberOfEnemies;
    public int spawnLocation;
    public float timeBetweenSpawn;
}