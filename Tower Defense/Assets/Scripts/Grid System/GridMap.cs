using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public event EventHandler OnLoaded;

    private CustomGrid<GridMapObject> grid;

    public GridMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new CustomGrid<GridMapObject>(width, height, cellSize, originPosition, (CustomGrid<GridMapObject> g, int x, int y) => new GridMapObject(g, x, y));
    }

    public void SetGridMapType(Vector3 worldPosition, GridMapObject.GridMapType gridMapType)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);
        if (gridMapObject != null)
            gridMapObject.SetGridMapType(gridMapType);
    }

    public GridMapObject.GridMapType GetGridMapType(Vector3 worldPosition)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);
        GridMapObject.GridMapType gridMapType = GridMapObject.GridMapType.None;

        if (gridMapObject != null)
            gridMapType = gridMapObject.GetGridMapType();

        return gridMapType;
    }

    public void SetGridMapTowerPlaced(Vector3 worldPosition, TowerController towerController)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);
        if (gridMapObject != null)
            gridMapObject.SetTowerPlaced(towerController);
    }

    public TowerController GetGridMapTowerPlaced(Vector3 worldPosition)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);
        TowerController towerController = null;

        if (gridMapObject != null)
            towerController = gridMapObject.GetTowerPlaced();

        return towerController;
    }

    public Vector3 GetGridCenterWorldPosition(Vector3 worldPosition)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);
        Vector3 centerPosition = Vector3.negativeInfinity;

        if (gridMapObject != null)
            centerPosition = gridMapObject.GetGridCenterWorldPosition();

        return centerPosition;
    }

    public void GetGridCoordinates(Vector3 worldPosition, out int x, out int y)
    {
        GridMapObject gridMapObject = grid.GetGridObject(worldPosition);

        if (gridMapObject != null)
            gridMapObject.GetGridCoordinates(out x, out y);
        else
        {
            x = -1;
            y = -1;
        }
    }

    public void SetGridMapVisual(GridMapVisual gridMapVisual)
    {
        gridMapVisual.SetGrid(this, grid);
    }

    public void Save()
    {
        List<GridMapObject.SaveObject> gridMapObjectSaveObjectList = new List<GridMapObject.SaveObject>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                GridMapObject gridMapObject = grid.GetGridObject(x, y);
                gridMapObjectSaveObjectList.Add(gridMapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { gridMapObjectSaveObjectArray = gridMapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
#if UNITY_EDITOR
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
#else
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>("level");
#endif

        foreach (GridMapObject.SaveObject gridMapObjectSaveObject in saveObject.gridMapObjectSaveObjectArray)
        {
            GridMapObject gridMapObject = grid.GetGridObject(gridMapObjectSaveObject.x, gridMapObjectSaveObject.y);
            gridMapObject.Load(gridMapObjectSaveObject);
        }

        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public class SaveObject
    {
        public GridMapObject.SaveObject[] gridMapObjectSaveObjectArray;
    }

    public class GridMapObject
    {
        public enum GridMapType
        {
            None,
            Grass,
            Path
        }

        private CustomGrid<GridMapObject> grid;
        private int x;
        private int y;
        private GridMapType gridMapType;
        private TowerController towerPlaced;

        public GridMapObject(CustomGrid<GridMapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public GridMapType GetGridMapType()
        {
            return gridMapType;
        }

        public void SetGridMapType(GridMapType gridMapType)
        {
            this.gridMapType = gridMapType;
            grid.TriggerGridObjectChanged(x, y);
        }

        public TowerController GetTowerPlaced()
        {
            return towerPlaced;
        }

        public void SetTowerPlaced(TowerController tower)
        {
            towerPlaced = tower;
        }

        public Vector3 GetGridCenterWorldPosition()
        {
            return grid.GetWorldPosition(x, y) + new Vector3(1, 1) * grid.GetCellSize() / 2f;
        }

        public void GetGridCoordinates(out int x, out int y)
        {
            x = this.x;
            y = this.y;
        }

        public override string ToString()
        {
            return gridMapType.ToString();
        }

        [System.Serializable]
        public class SaveObject
        {
            public GridMapType gridMapType;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                gridMapType = gridMapType,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            gridMapType = saveObject.gridMapType;
        }
    }
}
