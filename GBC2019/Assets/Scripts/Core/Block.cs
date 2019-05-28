using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A group of cells moving as one unit on a grid
/// </summary>
public class Block : MonoBehaviour
{
    public enum CellType { Player = 0, Enemy = 1, PowerUp = 2 };
    public enum CellSubType { Default, R, G, B, Y };

    private CellType blockType;
    private CellSubType blockSubType;
    private GridManager grid;
    [SerializeField]
    private Shape shape;

    // Properties
    [SerializeField]
    IResetProperty[] resetProperties;
    [SerializeField]
    IPreTransformCellProperty[] preTransformCellProperties;
    [SerializeField]
    IPreTransformBlockProperty[] preTransformBlockProperties;
    [SerializeField]
    IPostTransformCellProperty[] postTransformCellProperties;
    [SerializeField]
    IPostTransformBlockProperty[] postTransformBlockProperties;
    [SerializeField]
    IOnKillProperty[] onKillProperties;

    // Property references
    public AbsorbData absorbData { get; private set; }
    public AbsorbMatchingSubtype absorbMatchingSubtype { get; private set; }
    public AbsorbableByMatchingSubType absorbableByMatchingSubType { get; private set; }
    public KillNonMatchingSubType killNonMatchingSubType { get; private set; }
    public KillableByNonMatchingSubType killableByNonMatchingSubType { get; private set; }

    //Events
    public UnityEvent OnSetSubType;

    // Initialize lists
    List<Cell> cellsToAdd = new List<Cell>();
    List<Cell> overlappingCells = new List<Cell>();
    List<Cell> connected = new List<Cell>();
    List<Cell> disconnected = new List<Cell>();

    private void Awake()
    {
        // Attach this to a predefined object for scene management purposes if possible
        if (GameObject.Find("Blocks"))
        {
            transform.parent = GameObject.Find("Blocks").transform;
        }
    }

    /// <summary>
    /// Initializations on block creation
    /// <remark>Don't depend on start / awake</remark>
    /// </summary>
    private void RegisterProperties()
    {
        // arrays
        resetProperties = GetComponents<IResetProperty>();
        preTransformCellProperties = GetComponents<IPreTransformCellProperty>();
        preTransformBlockProperties = GetComponents<IPreTransformBlockProperty>();
        postTransformCellProperties = GetComponents<IPostTransformCellProperty>();
        postTransformBlockProperties = GetComponents<IPostTransformBlockProperty>();
        onKillProperties = GetComponents<IOnKillProperty>();
        // individuals
        absorbData = GetComponent<AbsorbData>();
        absorbMatchingSubtype = GetComponent<AbsorbMatchingSubtype>();
        absorbableByMatchingSubType = GetComponent<AbsorbableByMatchingSubType>();
        killNonMatchingSubType = GetComponent<KillNonMatchingSubType>();
        killableByNonMatchingSubType = GetComponent<KillableByNonMatchingSubType>();
        // Initialize
        foreach(IRegisterProperty registerProperty in GetComponents<IRegisterProperty>())
        {
            registerProperty.Register(this);
        }
    }

    public void SetGrid(GridManager _grid)
    {
        grid = _grid;
    }

    public GridManager GetGrid()
    {
        return grid;
    }

    public void CreateNewBlockShape(Shape _shape)
    {
        // assign shape
        shape = _shape;

        // assign shape callbacks
        shape.OnAddCell += SetCellParent;
        shape.OnSetSecondaryParameters += UpdatePositionUsingShapeAABB;
        shape.OnRemoveLastCell += Kill;

        // Initialization
        RegisterProperties();
    }

    public Shape GetShape()
    {
        return shape;
    }

    public void SetBlockType(CellType type)
    {
        blockType = type;
    }

    public CellType GetBlockType()
    {
       return blockType;
    }

    public void SetBlockSubType(CellSubType type)
    {
        blockSubType = type;
        if (OnSetSubType != null)
        {
            OnSetSubType.Invoke();
        }
    }

    public CellSubType GetBlockSubType()
    {
        return blockSubType;
    }

    /// <summary>
    /// Translate the block in a given direction
    /// </summary>
    /// <param name="move"></param>
    /// <returns>If the move was possible</returns>
    public bool Translate(Vector2Int move)
    {
        // Reset properties
        foreach(IResetProperty reset in resetProperties)
        {
            reset.Reset();
        }

        // Pre-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPreTransformCellProperty pre in preTransformCellProperties)
            {
                if (!pre.PreTransform(cell, cell.GetGridPosition() + move))
                {
                    return false;
                }
            }
        }

        // Pre-move
        foreach (IPreTransformBlockProperty pre in preTransformBlockProperties)
        {
            if (!pre.PreTransform(move))
            {
                return false;
            }
        }

        // Move
        shape.Translate(move);

        // Post-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPostTransformCellProperty post in postTransformCellProperties)
            {
                post.PostTransform(cell);
            }
        }

        // Post-move
        foreach (IPostTransformBlockProperty post in postTransformBlockProperties)
        {
            post.PostTransform();
        }

        return true;
    }

    /// <summary>
    /// Translate block in a given direction
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Translate(int x, int y)
    {
        return Translate(new Vector2Int(x, y));
    }

    /// <summary>
    /// Set block position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool SetPosition(Vector2Int position)
    {
        return Translate(position - shape.aabbCenter);
    }

    /// <summary>
    /// Set block position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool SetPosition(int x, int y)
    {
        return SetPosition(new Vector2Int(x, y));
    }

    /// <summary>
    /// Rotate the block
    /// </summary>
    /// <param name="direction"> +1 for clockwise, -1 for anti-clockwise; by 90 degrees</param>
    /// <returns></returns>
    public bool Rotate(int direction)
    {
        // Reset properties
        foreach (IResetProperty reset in resetProperties)
        {
            reset.Reset();
        }

        // Pre-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPreTransformCellProperty pre in preTransformCellProperties)
            {
                if (!pre.PreTransform(cell, shape.RotateRelative(cell.GetGridPosition(), direction)))
                {
                    return false;
                }
            }
        }

        // Pre-move
        foreach (IPreTransformBlockProperty pre in preTransformBlockProperties)
        {
            if (!pre.PreTransform(new Vector2Int(0, 0)))
            {
                return false;
            }
        }

        // Move
        shape.Rotate(direction);

        // Post-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPostTransformCellProperty post in postTransformCellProperties)
            {
                post.PostTransform(cell);
            }
        }

        // Post-move
        foreach (IPostTransformBlockProperty post in postTransformBlockProperties)
        {
            post.PostTransform();
        }

        return true;
    }

    private void UpdatePositionUsingShapeAABB()
    {
        transform.position = (shape.aabbCenterAbsolute +  new Vector2(0.5f, 0.5f)) * grid.unitLength;
    }

    /// <summary>
    /// Consume another block
    /// </summary>
    /// <param name="other"></param>
    public void Add(Block other)
    {
        // Clear the lists
        cellsToAdd.Clear();
        overlappingCells.Clear();

        CellGroup group;
        bool overlapping;
        Cell cell;
        
        // Go through all of the other block's cells
        for (int i = other.GetShape().cellList.Count - 1; i >= 0; i--)
        {
            cell = other.GetShape().cellList[i];
            // Check for overlaps
            overlapping = false;
            group = grid.GetCellGroup(cell.GetGridPosition());
            if (group != null && group.cells.Count > 1)
            {
                foreach (Cell groupCell in group.cells)
                {
                    if (groupCell.GetParentBlock() == this)
                    {
                        overlapping = true;
                        // If overlapping add to overlapping list
                        overlappingCells.Add(cell);
                        break;
                    }
                }
            }

            if (!overlapping)
            {
                // Otherwise add to the main list
                cellsToAdd.Add(cell);
            }
        }

        // Remove the main ones from other
        other.GetShape().RemoveCells(cellsToAdd);
        // And add to this
        shape.AddCells(cellsToAdd);
        foreach (Cell overlap in overlappingCells)
        {
            // Kill the overlapping cells
            overlap.Kill();
        }

        // Clear the lists
        cellsToAdd.Clear();
        overlappingCells.Clear();
    }

    private void SetCellParent(Cell cell)
    {
        cell.SetParentBlock(this);
    }

    private void Kill()
    {
        foreach(IOnKillProperty onKillProperty in onKillProperties)
        {
            onKillProperty.OnKill();
        }
    }

    /// <summary>
    /// Kill cells not connected by a valid path to the shape's center of mass
    /// </summary>
    public void KillDisconnectedCells()
    {
        connected = shape.ConnectedCells();
        disconnected.Clear();

        foreach (Cell cell in shape.cellList)
        {
            if (cell != null && !connected.Contains(cell))
            {
                disconnected.Add(cell);
            }
        }

        foreach (Cell cell in disconnected)
        {
            if (cell != null)
            {
                cell.Kill();
            }
        }

        connected.Clear();
        disconnected.Clear();
    }

    /// <summary>
    /// Destroy this block and all the cells
    /// </summary>
    public void Annhilate()
    {
        // Just remove all the cells from the shape
        List<Cell> cellList = new List<Cell>(shape.cellList);
        shape.RemoveCells(cellList);
    }
}
