using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : SingletonMonobehaviour<GridManager>
{

    public int numOfRows = 50, numOfColumns = 50;

    public float gridCellSize = 1.0f, gridCellWidth = 128.0f, gridCellHeight = 96.0f;

    public float halfGridCellWidth = 64.0f, halfGridCellHeight = 48.0f;

    public bool showGrid = false, showNodes = false, showLabels = false, showObstacleBlocks = false, nodeLabelReady = false;

    public GameObject gridNoLabel = null;
    public GameObject gridLabels = null;

    public Vector3 origin = new Vector3(0.0f, -2400.0f, 0f);
    private GameObject[] obstacleArray = new GameObject[0];

    public Grid[,] grids { get; set; }

    private bool nodesReady = false;

    Transform myTransform = null;

    public void Init()
    {
        myTransform = transform;
        myTransform.position = origin;

        CreateNodeGrid();


    }

    private void CreateNodeGrid()
    {
        grids = new Grid[numOfColumns, numOfRows];
        int index = 0;
        for (int i = 0; i < numOfColumns; ++i)
        {
            for (int j = 0; j < numOfRows;++j)
            {
                Vector3 gridPosition = GetGridCenterPosition(index);
                Grid grid = new Grid(gridPosition);
                grid.SetColRow(i, j);
                grids[i, j] = grid;

                GameObject gridObj = new GameObject(i.ToString() + "_" + j.ToString() + "_" + index.ToString());
                gridObj.transform.position = gridPosition;
                gridObj.transform.localScale = Vector3.one;
                gridObj.transform.rotation = Quaternion.identity;
                gridObj.transform.SetParent(myTransform);
                gridObj.layer = LayerMask.NameToLayer("Grid");

                PolygonCollider2D _collider2D = gridObj.AddComponent<PolygonCollider2D>();
                Vector2[] points = new Vector2[] {
                    new Vector2(0.0f,halfGridCellHeight),
                    new Vector2(0.0f,0.0f),
                    new Vector2(0.0f,halfGridCellHeight),
                    new Vector2(-halfGridCellWidth,0.0f),
                    new Vector2(0.0f,-halfGridCellHeight),
                    new Vector2(halfGridCellWidth,0.0f)
                };
                _collider2D.points = points;
                _collider2D.isTrigger = true;

                index++;
            }
        }
    }

    public Vector3 GetGridCenterPosition(int index)
    {
        Vector3 gridPosition = GetGridPosition(index);
        gridPosition.y += halfGridCellHeight;
        return gridPosition;
    }

    public Vector3 GetGridPosition(int index)
    {
        int row = index % numOfRows;
        int col = index / numOfColumns;

        float xPosition = col * halfGridCellWidth - row * halfGridCellWidth;
        float yPosition = row * halfGridCellHeight + col * halfGridCellHeight;

        return myTransform.position + new Vector3(xPosition, yPosition, 0.0f);

    }



}


