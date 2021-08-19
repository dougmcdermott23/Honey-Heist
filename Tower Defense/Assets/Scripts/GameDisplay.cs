using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_Text nextLevelText;
    [SerializeField] private TMP_Text upgradeDamageText;
    [SerializeField] private TMP_Text upgradeRangeText;
    [SerializeField] private TMP_Text upgradeSpeedText;
    [SerializeField] private TMP_Text placeTowerText;

    private const string winText = "WIN";
    private const string loseText = "GAME OVER";
    private const string damageText = "UPGRADE DAMAGE (Z):\n";
    private const string rangeText = "UPGRADE RANGE (X):\n";
    private const string speedText = "UPGRADE SPEED (C):\n";

    public void Init(int currencyAmount, int healthAmount)
    {
        currencyText.text = currencyAmount.ToString();
        healthText.text = healthAmount.ToString();

        GameController.instance.OnCurrencyChange += GameController_OnCurrencyChange;
        GameController.instance.OnHealthChange += GameController_OnHealthChange;
        GameController.instance.OnGameOverState += Instance_OnGameOverState;
        GameController.instance.OnChangeLevelState += GameController_OnChangeLevelState;
        GameController.instance.OnPlayerPlaceTower += Instance_OnPlayerPlaceTower;
        GameController.instance.OnTowerUpgrade += Instance_OnTowerUpgrade;
    }

    private void GameController_OnCurrencyChange(object sender, int amount)
    {
        currencyText.text = amount.ToString();
    }

    private void GameController_OnHealthChange(object sender, int amount)
    {
        healthText.text = amount.ToString();
    }

    private void Instance_OnGameOverState(object sender, bool win)
    {
        if (win)
            gameOverText.text = (winText);
        else
            gameOverText.text = (loseText);

        gameOverText.gameObject.SetActive(true);
        nextLevelText.gameObject.SetActive(true);
    }

    private void GameController_OnChangeLevelState(object sender, bool isEndOfLevel)
    {
        nextLevelText.gameObject.SetActive(isEndOfLevel);
    }

    private void Instance_OnPlayerPlaceTower(object sender, bool towerPlaced)
    {
        placeTowerText.gameObject.SetActive(!towerPlaced);
    }

    private void Instance_OnTowerUpgrade(object sender, GameController.OnTowerUpgradeEventArgs upgrade)
    {
        switch (upgrade.type)
        {
            case TowerController.TowerUpgradeType.Damage:
                upgradeDamageText.text = string.Format("{0}{1}", damageText, upgrade.cost);
                break;
            case TowerController.TowerUpgradeType.Range:
                upgradeRangeText.text = string.Format("{0}{1}", rangeText, upgrade.cost);
                break;
            case TowerController.TowerUpgradeType.Speed:
                upgradeSpeedText.text = string.Format("{0}{1}", speedText, upgrade.cost);
                break;
            default:
                break;
        }
    }
}
