using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerProperty
{
    private float upgradeCost;
    public int UpgradeCost
    {
        get => (int)upgradeCost;
        private set { upgradeCost = value; }
    }
    private float upgradeModifier;

    public TowerProperty(int startingUpgradeCost, float upgradeCostModifier)
    {
        UpgradeCost = startingUpgradeCost;
        upgradeModifier = upgradeCostModifier;
    }

    public void IncreaseUpgradeCost()
    {
        upgradeCost *= upgradeModifier;
    }
}

public class TowerProperty<T> : TowerProperty
{
    public T CurrentAmount { get; set; }
    public T UpgradeAmount { get; private set; }

    public TowerProperty(T startingAmount, T upgradeAmount, int startingUpgradeCost, float upgradeCostModifier) : base(startingUpgradeCost, upgradeCostModifier)
    {
        CurrentAmount = startingAmount;
        UpgradeAmount = upgradeAmount;
    }
}
