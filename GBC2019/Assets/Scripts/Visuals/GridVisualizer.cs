using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Maintains the visuals for a given grid
// </summary>
public class GridVisualizer : MonoBehaviour
{
    public GridManager grid;
    public GameObject gridLine;

    //private void Start()
    //{
    //    grid.InitializeGrid();
    //    SpawnGridVisuals();
    //}

    public void SpawnGridVisuals()
    {
        LineRenderer line;
        for(int i = 0; i < grid.GetHeight(); i++)
        {
            line = Instantiate(gridLine, grid.origin + i * grid.unitLength * Vector3.up, Quaternion.identity).GetComponent<LineRenderer>();
            line.SetPosition(1, grid.unitLength * grid.GetWidth() * 0.5f * Vector3.right);
            line.SetPosition(2, grid.unitLength * grid.GetWidth() * Vector3.right);
        }

        for (int i = 0; i < grid.GetWidth(); i++)
        {
            line = Instantiate(gridLine, grid.origin + i * grid.unitLength * Vector3.right, Quaternion.identity).GetComponent<LineRenderer>();
            line.SetPosition(1, grid.unitLength * grid.GetHeight() * 0.5f * Vector3.up);
            line.SetPosition(2, grid.unitLength * grid.GetHeight() * Vector3.up);
        }
    }   
}
