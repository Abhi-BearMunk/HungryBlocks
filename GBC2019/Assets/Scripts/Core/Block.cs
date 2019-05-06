using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum CellType { Player, Enemy };
    public enum CellSubType { Default, R, G, B, Y };

    private CellType blockType;
    private CellSubType blockSubType;
    private GridManager grid;
    [SerializeField]
    private Shape shape;

    private void Awake()
    {
        // Attach this to a predefined object for scene management purposes if possible
        if (GameObject.Find("Blocks"))
        {
            transform.parent = GameObject.Find("Blocks").transform;
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
        shape = _shape;
        shape.OnAddCell += SetCellParent;
        shape.OnSetSecondaryParameters += UpdatePositionUsingShapeAABB;
        shape.OnRemoveLastCell += Kill;
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
        //// Check if move is possible
        //foreach(Cell cell in shape.cellList)
        //{
        //    foreach (ICheckBlockedProperty blockedChecker in GetComponents<ICheckBlockedProperty>())
        //    {
        //        if (blockedChecker.IsBlocked(cell.GetGridPosition() + move))
        //        {
        //            return false;
        //        }
        //    }
        //}

        // Reset properties
        foreach(IResetProperty reset in GetComponents<IResetProperty>())
        {
            reset.Reset();
        }

        // Pre-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPreTransformCellProperty pre in GetComponents<IPreTransformCellProperty>())
            {
                if (!pre.PreTransform(cell, cell.GetGridPosition() + move))
                {
                    return false;
                }
            }
        }

        // Pre-move
        foreach (IPreTransformBlockProperty pre in GetComponents<IPreTransformBlockProperty>())
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
            foreach (IPostTransformCellProperty post in GetComponents<IPostTransformCellProperty>())
            {
                post.PostTransform(cell);
            }
        }

        // Post-move
        foreach (IPostTransformBlockProperty post in GetComponents<IPostTransformBlockProperty>())
        {
            post.PostTransform();
        }

        return true;
    }

    public bool Translate(int x, int y)
    {
        return Translate(new Vector2Int(x, y));
    }

    public bool SetPosition(Vector2Int position)
    {
        return Translate(position - shape.aabbCenter);
    }

    public bool SetPosition(int x, int y)
    {
        return SetPosition(new Vector2Int(x, y));
    }

    private void UpdatePositionUsingShapeAABB()
    {
        transform.position = (shape.aabbCenterAbsolute +  new Vector2(0.5f, 0.5f)) * grid.unitLength;
    }

    public void Add(Block other)
    {
        //if(other && other.GetComponent<BlockMover>())
        //{
        //    other.GetComponent<BlockMover>().velocity = GetComponent<BlockMover>().velocity;
        //}
        List<Cell> cellsToAdd = new List<Cell>();
        List<Cell> overlappingCells = new List<Cell>();

        CellGroup group;
        bool overlapping;
        Cell cell;
        
        for (int i = other.GetShape().cellList.Count - 1; i >= 0; i--)
        {
            cell = other.GetShape().cellList[i];
            overlapping = false;
            group = grid.GetCellGroup(cell.GetGridPosition());
            if (group != null && group.cells.Count > 1)
            {
                foreach (Cell groupCell in group.cells)
                {
                    if (groupCell.GetParentBlock() == this)
                    {
                        overlapping = true;
                        overlappingCells.Add(groupCell);
                        break;
                    }
                }
            }

            if (!overlapping)
            {
                //other.GetShape().RemoveCell(cell);
                cellsToAdd.Add(cell);
            }
        }

        other.GetShape().RemoveCells(cellsToAdd);
        shape.AddCells(cellsToAdd);
        //other.GetShape().RemoveCells(cellsToAdd);
        foreach (Cell overlap in overlappingCells)
        {
            overlap.Kill();
        }
    }

    void SetCellParent(Cell cell)
    {
        cell.SetParentBlock(this);
    }
    
    void Kill()
    {
        foreach(IOnKillProperty onKillProperty in GetComponents<IOnKillProperty>())
        {
            onKillProperty.OnKill();
        }
    }
}
