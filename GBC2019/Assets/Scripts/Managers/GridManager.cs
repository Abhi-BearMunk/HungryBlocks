using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BlockEvent : UnityEvent<Block>
{
}

// <summary>
// A Singleton class containing data for the level grid
// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width = 64;
    public int GetWidth() { return width; }
    [SerializeField]
    private int height = 36;
    public int GetHeight() { return height; }
    private List<CellGroupRow> grid = new List<CellGroupRow>();
    public UnityEvent OnInitializeGrid;

    public GameObject cellPrefab;
    public BlockEvent OnCreateBlock;

    // Bottom left of the grid
    public Vector3 origin;
    // Length of one cell
    public float unitLength = 1;


    //private static GridManager instance;

    //private void Awake()
    //{
    //    instance = this;
    //}

    //// <summary>
    //// Singleton Instance of GridManager
    //// </summary>
    //public static GridManager Instance()
    //{
    //    if(instance == null)
    //    {
    //        Debug.LogWarning("Grid Manager not found! Creating a new one with default values");            
    //        new GameObject("Grid Manager", typeof(GridManager));
    //    }
    //    return instance;
    //}

    // <summary>
    // Create rows to initialize the grid
    // </summary>
    public void InitializeGrid()
    {
        for(int i = 0; i < height; i++)
        {
            grid.Add(new CellGroupRow(width, height));
        }
        OnInitializeGrid.Invoke();
    }

    // <summary>
    // Create a new Block of a certain shape and type
    // </summary>
    public Block CreateBlock(List<Vector2Int> shapeDefinition, Cell.CellType type = Cell.CellType.R)
    {
        // Create the block and set refernces
        GameObject blockObject = new GameObject();
        blockObject.AddComponent<Block>();
        Block block = blockObject.GetComponent<Block>();
        block.grid = this;
        block.SetBlockType(type);

        // Create a list to store the generated cells
        List<Cell> cells = new List<Cell>();
        GameObject temp;
        Cell tempCell;
        foreach (Vector2Int pos in shapeDefinition)
        {
            temp = Instantiate(cellPrefab);
            if (!temp.GetComponent<Cell>())
            {
                temp.AddComponent<Cell>();
            }
            tempCell = temp.GetComponent<Cell>();
            // Set type
            tempCell.SetCellType(type);
            // Set Grid
            tempCell.SetGrid(this);
            // Set position
            tempCell.SetGridPosition(pos);
            // Set parent
            tempCell.SetParentBlock(block);
            cells.Add(tempCell);
        }
        block.CreateNewBlockShape(new Shape(cells));
        OnCreateBlock.Invoke(block);
        return block;
    }

    // <summary>
    // Get the cells at a certain row and column
    // </summary>
    public CellGroup GetCellGroup(int row, int column)
    {
        if(OutOfBounds(row, column))
        {
            return null;
        }
        return grid[row].GetCellGroup(column);
    }

    // <summary>
    // Move Cell to a new CellGroup
    // </summary>
    public void TryChangeCellGroup(Cell cell, Vector2Int position)
    {
        if(!OutOfBounds(cell.GetGridPosition()))
        {
            RemoveCell(cell.GetGridPosition(), cell);
        }
        if(!OutOfBounds(position))
        {
            AddCell(position, cell);
        }
    }

    // <summary>
    // Add a cell at a certain row and column
    // </summary>
    void AddCell(int row, int column, Cell cell)
    {
        grid[row].AddCell(column, cell);
    }

    // <summary>
    // Add a cell at a certain row and column
    // </summary>
    void AddCell(Vector2Int position, Cell cell)
    {
        grid[position.y].AddCell(position.x, cell);
    }

    // <summary>
    // Remove a cell from a certain row and column
    // </summary>
    bool RemoveCell(int row, int column, Cell cell)
    {
        return grid[row].RemoveCell(column, cell);
    }

    // <summary>
    // Remove a cell from a certain row and column
    // </summary>
    bool RemoveCell(Vector2Int position, Cell cell)
    {
        return grid[position.y].RemoveCell(position.x, cell);
    }

    // <summary>
    // Check if a given grid position is out of bounds
    // </summary>
    public bool OutOfBounds(int row, int column)
    {
        return column < 0 || column >= width || row < 0 || row >= height;
    }

    // <summary>
    // Check if a given grid position is out of bounds
    // </summary>
    public bool OutOfBounds(Vector2Int position)
    {
        return position.x < 0 || position.x >= width || position.y < 0 || position.y >= height;
    }
}
