using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Utils;

[RequireComponent(typeof(GameController))]
public class InputController : MonoBehaviour
{
    private GameController gameController;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const KeyCode UPGRADE_TOWER_ONE_KEY_CODE = KeyCode.Z;
    private const KeyCode UPGRADE_TOWER_TWO_KEY_CODE = KeyCode.X;
    private const KeyCode UPGRADE_TOWER_THREE_KEY_CODE = KeyCode.C;
    private const KeyCode PLAYER_SELECT_KEY_CODE = KeyCode.Space;
    private const KeyCode RETURN_KEY_CODE = KeyCode.Return;
    private const KeyCode EDIT_KEY_CODE = KeyCode.E;
    private const KeyCode GRASS_TYPE_KEY_CODE = KeyCode.Alpha1;
    private const KeyCode PATH_TYPE_KEY_CODE = KeyCode.Alpha2;

    private void Start()
    {
        gameController = GetComponent<GameController>();
    }

    private void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw(HORIZONTAL), Input.GetAxisRaw(VERTICAL));
        gameController.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(PLAYER_SELECT_KEY_CODE))
        {
            gameController.SelectGridSpace();
        }

        if (Input.GetKeyDown(UPGRADE_TOWER_ONE_KEY_CODE))
        {
            gameController.UpgradeTower(TowerController.TowerUpgradeType.Damage);
        }

        if (Input.GetKeyDown(UPGRADE_TOWER_TWO_KEY_CODE))
        {
            gameController.UpgradeTower(TowerController.TowerUpgradeType.Range);
        }

        if (Input.GetKeyDown(UPGRADE_TOWER_THREE_KEY_CODE))
        {
            gameController.UpgradeTower(TowerController.TowerUpgradeType.Speed);
        }

        if (Input.GetKeyDown(RETURN_KEY_CODE))
        {
            gameController.StartNextLevel();
        }

        if (Input.GetKeyDown(EDIT_KEY_CODE))
        {
            gameController.SetEditMode();
        }

        if (Input.GetMouseButton(0))
        {
            GridMap.GridMapObject.GridMapType gridMapType = GridMap.GridMapObject.GridMapType.None;

            if (Input.GetKey(GRASS_TYPE_KEY_CODE))
            {
                gridMapType = GridMap.GridMapObject.GridMapType.Grass;
            }
            else if (Input.GetKey(PATH_TYPE_KEY_CODE))
            {
                gridMapType = GridMap.GridMapObject.GridMapType.Path;

            }

            Vector3 position = UtilsClass.GetMouseWorldPosition();
            gameController.SetGridMapType(position, gridMapType);
        }

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    gameController.SaveGridMap();
        //}

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    gameController.LoadGridMap();
        //}
    }
}
