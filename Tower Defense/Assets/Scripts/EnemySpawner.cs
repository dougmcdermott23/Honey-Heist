using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public event EventHandler OnEndOfWave;

    [SerializeField] private ObjectFactory enemyFactory;
    [SerializeField] private Transform enemiesStorage;
    [SerializeField] private PointList localWaypoints;
    private List<EnemyWave> enemyWaveList;
    private Vector3[][] globalWaypoints;

    private EnemyWave currentWave;
    private bool spawning;
    private int enemiesSpawned;
    private float spawnTimer;

    private void Start()
    {
        enemyFactory.Init();

        globalWaypoints = new Vector3[localWaypoints.list.Count][];
        for (int i = 0; i < localWaypoints.list.Count; i++)
        {
            globalWaypoints[i] = new Vector3[localWaypoints.list[i].array.Length];
            for (int j = 0; j < localWaypoints.list[i].array.Length; j++)
            {
                globalWaypoints[i][j] = localWaypoints.list[i].array[j] + transform.position;
            }
        }
    }

    private void Update()
    {
        if (spawning)
            SpawnWave();
    }

    public void InitSpawnWave(int waveIndex, List<EnemyWave> waveList)
    {
        enemyWaveList = waveList;

        if (waveIndex >= enemyWaveList.Count || spawning)
            return;

        Debug.Log(string.Format("Spawning wave: {0}", waveIndex));

        currentWave = enemyWaveList[waveIndex];
        enemiesSpawned = 0;
        spawnTimer = currentWave.timeBetweenSpawn;

        spawning = true;
    }

    public void StopSpawning()
    {
        spawning = false;
        spawnTimer = 0;
    }

    private void SpawnWave()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            SpawnEnemy(currentWave.enemyType, currentWave.spawnLocation);
            enemiesSpawned++;
            spawnTimer = currentWave.timeBetweenSpawn;

            if (enemiesSpawned >= currentWave.numberOfEnemies)
            {
                spawning = false;
                OnEndOfWave?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void SpawnEnemy(EnemyProperties enemy, int spawnLocation)
    {
        var enemyInstance = enemyFactory.Get();
        if (enemiesStorage != null) enemyInstance.transform.parent = enemiesStorage;

        int spawnIndex = (spawnLocation < globalWaypoints.Length) ? spawnLocation : 0;

        EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();
        enemyController.Init(enemy, globalWaypoints[spawnIndex]);
        enemyController.OnEnemyDespawn += GameController.instance.EnemyController_OnEnemyDespawn;
        EnemyController.enemyList.Add(enemyController);
    }

    private void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.3f;
            
            for (int i = 0; i < localWaypoints.list.Count; i++)
            {
                for (int j = 0; j < localWaypoints.list[i].array.Length; j++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? (globalWaypoints[i][j]) : (localWaypoints.list[i].array[j] + transform.position);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }

                Gizmos.color = Color.green;
            }
        }
    }
}

[System.Serializable]
public class Point
{
    public Vector3[] array;
}

[System.Serializable]
public class PointList
{
    public List<Point> list;
}
