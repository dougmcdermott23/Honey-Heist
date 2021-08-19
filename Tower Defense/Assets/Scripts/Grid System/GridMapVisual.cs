using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapVisual : MonoBehaviour
{
    [System.Serializable]
    public struct GridMapTypeUV
    {
        public GridMap.GridMapObject.GridMapType gridMapType;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private GridMapTypeUV[] gridMapTypeUVArray;
    private CustomGrid<GridMap.GridMapObject> grid;
    private Mesh mesh;
    private bool updateMesh;
    private Dictionary<GridMap.GridMapObject.GridMapType, UVCoords> uvCoordsDictionary;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<GridMap.GridMapObject.GridMapType, UVCoords>();
        foreach (GridMapTypeUV gridMapTypeUV in gridMapTypeUVArray)
        {
            uvCoordsDictionary[gridMapTypeUV.gridMapType] = new UVCoords
            {
                uv00 = new Vector2(gridMapTypeUV.uv00Pixels.x / textureWidth, gridMapTypeUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(gridMapTypeUV.uv11Pixels.x / textureWidth, gridMapTypeUV.uv11Pixels.y / textureHeight)
            };
        }
    }

    public void SetGrid(GridMap gridMap, CustomGrid<GridMap.GridMapObject> setGrid)
    {
        grid = setGrid;
        UpdateGridMapVisual();

        grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
        gridMap.OnLoaded += GridMap_OnLoaded;
    }

    private void GridMap_OnLoaded(object sender, System.EventArgs e)
    {
        updateMesh = true;
    }

    private void Grid_OnGridObjectChanged(object sender, CustomGrid<GridMap.GridMapObject>.OnGridObjectChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateGridMapVisual();
        }
    }

    private void UpdateGridMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                GridMap.GridMapObject gridObject = grid.GetGridObject(x, y);
                GridMap.GridMapObject.GridMapType gridMapType = gridObject.GetGridMapType();
                Vector2 gridUV00, gridUV11;

                if (gridMapType == GridMap.GridMapObject.GridMapType.None)
                {
                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.zero;
                    quadSize = Vector3.zero;
                }
                else
                {
                    UVCoords uvCoords = uvCoordsDictionary[gridMapType];
                    gridUV00 = uvCoords.uv00;
                    gridUV11 = uvCoords.uv11;
                }

                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridUV00, gridUV11);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}
 