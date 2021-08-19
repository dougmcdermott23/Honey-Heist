using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tools.Utils;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public event EventHandler<bool> OnChangeLevelState;
    public event EventHandler<bool> OnGameOverState;
    public event EventHandler<bool> OnPlayerPlaceTower;
    public event EventHandler<OnTowerUpgradeEventArgs> OnTowerUpgrade;
    public event EventHandler<int> OnCurrencyChange;
    public event EventHandler<int> OnHealthChange;

    public class OnTowerUpgradeEventArgs : EventArgs
    {
        public TowerController.TowerUpgradeType type;
        public int cost;
    }

    // Player
    public GameObject playerPreFab;
    private PlayerController player;
    [SerializeField] private Vector3 playerStartPosition;
    [SerializeField] private GameObject gridSquareIndicator;

    // Game Objects
    private HealthSystem health;
    private EnemySpawner enemySpawner;
    private bool gameOver;
    private bool gameOverTriggered;
    private bool endOfLevel;
    private int levelIndex;
    private int waveIndex;
    [SerializeField] private int startingHealth;
    [SerializeField] private Level[] levelArray;

    // Grid
    private GridMap gridMap;
    private GridMap.GridMapObject.GridMapType gridMapType;
    [SerializeField] private GridMapVisual gridMapVisual;
    private int gridWidth = 20;
    private int gridHeight = 15;
    private float gridCellSize = 1f;
    private Vector3 gridOriginPosition = Vector3.zero;
    private bool editMode;

    // UI
    [SerializeField] private GameDisplay gameDisplay;

    private void Awake()
    {
        // Singleton Pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        SoundManager.Initialize();

        // Create Player
        if (playerPreFab != null)
        {
            GameObject playerInstance = Instantiate(playerPreFab, playerStartPosition, Quaternion.identity);
            player = playerInstance.GetComponent<PlayerController>();
        }
        else
            Debug.LogError("Player PreFab is null!");

        // Create Game Objects
        health = new HealthSystem(startingHealth);
        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.OnEndOfWave += EnemySpawner_OnEndOfWave;

        // UI
        gameDisplay.Init(player.Currency, health.GetCurrentHealth());

        // Create Grid
        gridMap = new GridMap(gridWidth, gridHeight, gridCellSize, gridOriginPosition);
        gridMap.SetGridMapVisual(gridMapVisual);
        gridMap.Load();

        // Set up camera to match grid
        CameraController orthoCam = Camera.main.GetComponent<CameraController>();
        orthoCam.UpdateCamera(gridWidth, gridHeight, gridCellSize, gridOriginPosition);

        // Set up grid square indicator
        gridSquareIndicator.transform.localPosition = new Vector3(0, 0 - 1f);
        gridSquareIndicator.transform.localScale = Vector3.one * gridCellSize;
        gridSquareIndicator.SetActive(player.Tower);
        OnPlayerPlaceTower += (object sender, bool towerPlaced) =>
        {
            gridSquareIndicator.SetActive(!towerPlaced);
        };
    }

    private void Update()
    {
        UpdateGridSquareIndicatorPosition();

        CheckGameState();
    }

    private void CheckGameState()
    {
        bool alive = health.GetCurrentHealth() > 0;
        if (EnemyController.enemyList.Count > 0 && alive)
            return;

        if (gameOver)
        {
            if (gameOverTriggered)
            {
                OnGameOverState?.Invoke(this, alive);

                if (alive)
                    SoundManager.PlaySound(SoundManager.Sound.levelComplete);
                else
                    SoundManager.PlaySound(SoundManager.Sound.lose);

                gameOverTriggered = false;
                SoundManager.SetSoundEnabled(false);
            }
        } 
        else if (endOfLevel)
        {
            OnChangeLevelState?.Invoke(this, true);
            endOfLevel = false;

            SoundManager.PlaySound(SoundManager.Sound.levelComplete);
        }
    }

    private void EnemySpawner_OnEndOfWave(object sender, EventArgs e)
    {
        waveIndex++;

        if (waveIndex >= levelArray[levelIndex].enemyWaveList.Count)
        {
            endOfLevel = true;
            levelIndex++;

            if (levelIndex >= levelArray.Length)
                GameOver(true);
        }
        else
        {
            enemySpawner.InitSpawnWave(waveIndex, levelArray[levelIndex].enemyWaveList);
        }
    }

    private void InitLevel()
    {
        if (EnemyController.enemyList.Count > 0)
            return;

        if (levelIndex >= levelArray.Length)
        {
            Debug.Log("COMPLETED ALL LEVELS!");
            return;
        }

        Debug.Log(string.Format("STARTING LEVEL {0}", levelIndex));
        OnChangeLevelState?.Invoke(this, false);

        if (levelArray[levelIndex].enemyWaveList.Count <= 0)
        {
            Debug.Log("Wave list is empty");
            return;
        }

        waveIndex = 0;
        enemySpawner.InitSpawnWave(waveIndex, levelArray[levelIndex].enemyWaveList);
    }

    private void UpdateGridSquareIndicatorPosition()
    {
        if (player.Tower)
        {
            Vector3 position = gridMap.GetGridCenterWorldPosition(player.transform.position);
            if (position.magnitude != Mathf.Infinity)
                gridSquareIndicator.transform.position = new Vector3(position.x, position.y);
        }
    }

    private void GameOver(bool win)
    {
        gameOver = true;
        gameOverTriggered = true;
        enemySpawner.StopSpawning();
        waveIndex = 0;
        levelIndex = 0;
    }

    private void ResetScene()
    {
        EnemyController.enemyList.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartNextLevel()
    {
        if (gameOver)
            ResetScene();

        InitLevel();
    }

    public void SetDirectionalInput(Vector2 input)
    {
        player.MoveDirection = input;
    }

    public void SelectGridSpace()
    {
        Vector3 playerPosition = player.transform.position;
        TowerController playerTower = player.Tower;
        TowerController gridTower = gridMap.GetGridMapTowerPlaced(playerPosition);

        if (playerTower && !gridTower)
        {
            Vector3 position = gridMap.GetGridCenterWorldPosition(playerPosition);
            if (position.magnitude != Mathf.Infinity)
            {
                if (gridMap.GetGridMapType(playerPosition) != GridMap.GridMapObject.GridMapType.Grass)
                    return;

                playerTower.gameObject.SetActive(true);
                playerTower.transform.position = gridMap.GetGridCenterWorldPosition(playerPosition);

                gridMap.SetGridMapTowerPlaced(playerPosition, playerTower);
                player.Tower = null;

                OnPlayerPlaceTower?.Invoke(this, true);
            }
        }
        else if (!playerTower && gridTower)
        {
            gridTower.gameObject.SetActive(false);
            player.Tower = gridTower;
            gridMap.SetGridMapTowerPlaced(playerPosition, null);

            OnPlayerPlaceTower?.Invoke(this, false);
        }
    }

    public void UpgradeTower(TowerController.TowerUpgradeType upgrade)
    {
        player.UpgradeTower(upgrade);
        OnCurrencyChange?.Invoke(this, player.Currency);
        OnTowerUpgrade?.Invoke(this, new OnTowerUpgradeEventArgs { type = upgrade, cost = player.Tower.GetTowerUpgradeCost(upgrade) });
    }

    public void SetEditMode()
    {
#if UNITY_EDITOR
        editMode = !editMode;
        Debug.Log(string.Format("Edit Mode: {0}", editMode.ToString()));
#endif
    }

    public void EnemyController_OnEnemyDespawn(object sender, EnemyController.OnEnemyDespawnEventArgs e)
    {
        if (e.isDead)
        {
            player.AddCurrency(e.value);
            OnCurrencyChange?.Invoke(this, player.Currency);
            SoundManager.PlaySound(SoundManager.Sound.EnemyHit);
        }
        else
        {
            health.Damage(e.damage);
            if (health.GetCurrentHealth() <= 0 && !gameOver)
                GameOver(false);

            OnHealthChange?.Invoke(this, health.GetCurrentHealth());
        }
    }

    public void SetGridMapType(Vector3 position, GridMap.GridMapObject.GridMapType gridMapType)
    {
        if (!editMode)
            return;

        gridMap.SetGridMapType(position, gridMapType);
    }

    public void SaveGridMap()
    {
        gridMap.Save();
    }

    public void LoadGridMap()
    {
        gridMap.Load();
    }
}
