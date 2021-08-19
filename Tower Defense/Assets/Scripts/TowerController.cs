using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public enum TowerUpgradeType
    {
        Damage,
        Range,
        Speed
    }

    public GameObject projectile;

    [SerializeField] private ObjectFactory projectileFactory;
    [SerializeField] private Transform projectileStorage;

    [Header("Tower Damage")]
    [SerializeField] private int startingTowerDamage = 25;
    [SerializeField] private int towerDamageUpgradeAmount = 25;
    [Header("Tower Range")]
    [SerializeField] private float startingTowerRange = 10f;
    [SerializeField] private float towerRangeUpgradeAmount = 0.1f;
    [Header("Tower Speed")]
    [SerializeField] private float startingTowerSpeed = 0.5f;
    [SerializeField] private float towerSpeedUpgradeAmount = 0.1f;
    [Header("Tower Cost")]
    [SerializeField] private int startingTowerCost = 100;
    [SerializeField] private float towerUpgradeCostModifier = 2f;


    private float shootTimer;
    private TowerProperty<int> towerDamage;
    private TowerProperty<float> towerRange;
    private TowerProperty<float> towerSpeed;

    private void Awake()
    {
        projectileFactory.Init();

        towerDamage = new TowerProperty<int>(startingTowerDamage, towerDamageUpgradeAmount, startingTowerCost, towerUpgradeCostModifier);
        towerRange = new TowerProperty<float>(startingTowerRange, towerRangeUpgradeAmount, startingTowerCost, towerUpgradeCostModifier);
        towerSpeed = new TowerProperty<float>(startingTowerSpeed, towerSpeedUpgradeAmount, startingTowerCost, towerUpgradeCostModifier);
    }

    private void Update()
    {
        if (shootTimer <= 0)
        {
            CreateProjectile();

            shootTimer = towerSpeed.CurrentAmount;
        }
        else
        {
            shootTimer -= Time.deltaTime;
        }
    }

    private void CreateProjectile()
    {
        EnemyController enemy = GetClosestEnemy();

        if (enemy != null)
        {
            var projectileInstance = projectileFactory.Get();
            if (projectileStorage != null) projectileInstance.transform.parent = projectileStorage;

            ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();
            projectileController.Init(transform.position, Quaternion.identity, enemy, towerDamage.CurrentAmount);
        }
    }

    private EnemyController GetClosestEnemy()
    {
        return EnemyController.GetClosestEnemy(transform.position, towerRange.CurrentAmount);
    }

    public int UpgradeTower(TowerUpgradeType upgradeType, int playerCurrency)
    {
        int upgradeCost = 0;

        switch (upgradeType)
        {
            case TowerUpgradeType.Damage:
                upgradeCost = UpgradeTower(towerDamage, playerCurrency);
                if (upgradeCost > 0)
                    UpgradeTowerDamage();
                break;

            case TowerUpgradeType.Range:
                upgradeCost = UpgradeTower(towerRange, playerCurrency);
                if (upgradeCost > 0)
                    UpgradeTowerRange();
                break;

            case TowerUpgradeType.Speed:
                upgradeCost = UpgradeTower(towerSpeed, playerCurrency);
                if (upgradeCost > 0)
                    UpgradeTowerSpeed();
                break;

            default:
                break;
        }

        return upgradeCost;
    }

    public int UpgradeTower(TowerProperty towerProperty, int playerCurrency)
    {
        int upgradeCost = 0;

        if (playerCurrency >= towerProperty.UpgradeCost)
        {
            upgradeCost = towerProperty.UpgradeCost;
            towerProperty.IncreaseUpgradeCost();
        }

        return upgradeCost;
    }

    private void UpgradeTowerDamage()
    {
        towerDamage.CurrentAmount += towerDamage.UpgradeAmount;
    }

    private void UpgradeTowerRange()
    {
        towerRange.CurrentAmount *= (1 + towerRange.UpgradeAmount);
    }

    private void UpgradeTowerSpeed()
    {
        towerSpeed.CurrentAmount *= (1 - towerSpeed.UpgradeAmount);
    }

    public Vector3 GetTowerRange()
    {
        return Vector3.one * towerRange.CurrentAmount * 2f;
    }

    public int GetTowerUpgradeCost(TowerUpgradeType upgradeType)
    {
        int upgradeCost = 0;

        switch (upgradeType)
        {
            case TowerUpgradeType.Damage:
                upgradeCost = towerDamage.UpgradeCost;
                break;
            case TowerUpgradeType.Range:
                upgradeCost = towerRange.UpgradeCost;
                break;
            case TowerUpgradeType.Speed:
                upgradeCost = towerSpeed.UpgradeCost;
                break;
            default:
                break;
        }

        return upgradeCost;
    }
}
