using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float positionX;
    private float positionY;
    private float orthoSize;

    public void UpdateCamera(int gridWidth, int gridHeight, float gridCellSize, Vector3 gridOriginPosition)
    {
        float posX = (float)gridWidth / 2 * gridCellSize + gridOriginPosition.x;
        float posY = (float)gridHeight / 2 * gridCellSize + gridOriginPosition.y;
        transform.localPosition = new Vector3(posX, posY, -10f);

        Camera.main.orthographicSize = (float)gridHeight / 2 * gridCellSize;
    }
}
