using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// A Singleton class containing data for the level grid
// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width = 64;
    [SerializeField]
    private int height = 36;

    private static GridManager instance;
    private List<CellGroupRow> grid;

    private void Awake()
    {
        instance = this;
    }

    // <summary>
    // Singleton Instance of GridManager
    // </summary>
    public static GridManager Instance()
    {
        if(instance == null)
        {
            Debug.LogWarning("Grid Manager not found! Creating a new one with default values");            
            new GameObject("Grid Manager", typeof(GridManager));
        }
        return instance;
    }

    // <summary>
    // Create rows to initialize the grid
    // </summary>
    void InitializeGrid()
    {
        for(int i = 0; i < height; i++)
        {
            grid.Add(new CellGroupRow(width, height));
        }
    }

    // <summary>
    // Get the cells at a certain row and column
    // </summary>
    public CellGroup GetCellGroup(int row, int column)
    {
        return grid[row].GetCellGroup(column);
    }

    // <summary>
    // Get the cells at a certain row and column
    // </summary>
    public void AddCell(int row, int column, Cell cell)
    {
        grid[row].AddCell(column, cell);
    }

    // <summary>
    // Remove a cell from a certain row and column
    // </summary>
    public bool RemoveCell(int row, int column, Cell cell)
    {
        return grid[row].RemoveCell(column, cell);
    }

    // <summary>
    // Check if a given grid position is out of bounds
    // </summary>
    public bool OutOfBounds(int i, int j)
    {
        return i < 0 || i >= width || j < 0 || j >= height;
    }
}
