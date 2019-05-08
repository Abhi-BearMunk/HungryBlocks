using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BlockEvent : UnityEvent<Block>
{
}

/// <summary>
/// A class containing data for the level grid, and used to spawn new blocks
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int width = 64;
    public int GetWidth() { return width; }
    [SerializeField]
    private int height = 36;
    public int GetHeight() { return height; }
    public List<CellGroupRow> grid { get; private set; }
    public UnityEvent OnInitializeGrid;

    public GameObject cellPrefab;
    public BlockEvent OnCreateBlock;
    public List<Block> blocks = new List<Block>();

    // Bottom left of the grid
    public Vector3 origin;
    // Length of one cell
    public float unitLength = 1;

    // <summary>
    // Create rows to initialize the grid
    // </summary>
    public void InitializeGrid()
    {
        grid = new List<CellGroupRow>();
        for (int i = 0; i < height; i++)
        {
            grid.Add(new CellGroupRow(width, i));
        }
        OnInitializeGrid.Invoke();
    }

    /// <summary>
    /// Create a new Block of a certain shape and type
    /// </summary>
    public Block CreateBlock(List<Vector2Int> shapeDefinition, Vector2Int position, Block.CellType type = Block.CellType.Enemy, Block.CellSubType subtype = Block.CellSubType.Default, GameObject blockObject = null)
    {
        // Create the block and set refernces
        if (blockObject == null)
        {
            blockObject = new GameObject("Block");
        }
        if (blockObject.GetComponent<Block>() == null)
        {
            blockObject.AddComponent<Block>();
        }
        Block block = blockObject.GetComponent<Block>();
        block.SetGrid(this);
        block.SetBlockType(type);
        block.SetBlockSubType(subtype);

        // Create a list to store the generated cells
        List<Cell> cells = new List<Cell>();
        GameObject temp;
        Cell tempCell;
        foreach (Vector2Int pos in shapeDefinition)
        {
            temp = Instantiate(cellPrefab, (Vector3)((Vector2)(pos + position) * unitLength) + origin, Quaternion.identity);
            if (!temp.GetComponent<Cell>())
            {
                temp.AddComponent<Cell>();
            }
            tempCell = temp.GetComponent<Cell>();
            // Set parent
            tempCell.SetParentBlock(block);
            // Place on Grid
            tempCell.PlaceOnGrid();
            // Set position
            tempCell.SetGridPosition(pos);            
            cells.Add(tempCell);
        }
        block.CreateNewBlockShape(new Shape(cells));
        block.RegisterProperties();
        block.SetPosition(position);
        blocks.Add(block);
        OnCreateBlock.Invoke(block);
        return block;
    }

    /// <summary>
    /// Get the cells at a certain row and column
    /// </summary>
    public CellGroup GetCellGroup(int row, int column)
    {
        if(OutOfBounds(row, column))
        {
            return null;
        }
        return grid[row].GetCellGroup(column);
    }

    /// <summary>
    /// Get the cells at a certain position
    /// </summary>
    public CellGroup GetCellGroup(Vector2Int position)
    {
        return GetCellGroup(position.y, position.x);
    }

    /// <summary>
    /// Move Cell to a new CellGroup
    /// </summary>
    public void MoveCell(Cell cell, Vector2Int position)
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

    /// <summary>
    /// Add a cell at a certain row and column
    /// </summary>
    void AddCell(int row, int column, Cell cell)
    {
        grid[row].AddCell(column, cell);
    }

    /// <summary>
    /// Add a cell at a certain row and column
    /// </summary>
    void AddCell(Vector2Int position, Cell cell)
    {
        grid[position.y].AddCell(position.x, cell);
    }

    /// <summary>
    /// Remove a cell from a certain row and column
    /// </summary>
    bool RemoveCell(int row, int column, Cell cell)
    {
        //if(OutOfBounds(row, column))
        //{
        //    return false;
        //}
        return grid[row].RemoveCell(column, cell);
    }

    /// <summary>
    /// Remove a cell from a certain row and column
    /// </summary>
    public bool RemoveCell(Vector2Int position, Cell cell)
    {
        return RemoveCell(position.y, position.x, cell);
    }

    /// <summary>
    /// Check if a given grid position is out of bounds
    /// </summary>
    public bool OutOfBounds(int row, int column)
    {
        return column < 0 || column >= width || row < 0 || row >= height;
    }

    /// <summary>
    /// Check if a given grid position is out of bounds
    /// </summary>
    public bool OutOfBounds(Vector2Int position)
    {
        return position.x < 0 || position.x >= width || position.y < 0 || position.y >= height;
    }
}
