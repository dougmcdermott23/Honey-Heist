using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public GameObject towerPreFab;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int startingCurrency;

    private Rigidbody2D rb;
    private Transform towerRangeIndicator;
    private const string TOWER_RANGE_INDICATOR = "TowerRangeIndicator";

    public TowerController Tower { get; set; }
    public Vector2 MoveDirection { get; set; }
    public int Currency { get; private set; }

    private void Awake()
    {
        Tower = Instantiate(towerPreFab, Vector3.zero, Quaternion.identity).GetComponent<TowerController>();
        Tower.gameObject.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        towerRangeIndicator = transform.Find(TOWER_RANGE_INDICATOR);
        RefreshRangeIndicator();

        Currency = startingCurrency;
    }

    private void Update()
    {
        towerRangeIndicator.gameObject.SetActive(Tower);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + MoveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    public void UpgradeTower(TowerController.TowerUpgradeType upgrade)
    {
        if (!Tower)
            return;

        int upgradeCost = Tower.UpgradeTower(upgrade, Currency);
        Currency -= upgradeCost;
        if (upgradeCost > 0) SoundManager.PlaySound(SoundManager.Sound.upgradeTower);
        RefreshRangeIndicator();
    }

    private void RefreshRangeIndicator()
    {
        if (Tower)
            towerRangeIndicator.transform.localScale = Tower.GetTowerRange();
    }
}
