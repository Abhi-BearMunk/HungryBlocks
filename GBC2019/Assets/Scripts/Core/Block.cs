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

    public void Translate(Vector2Int move)
    {
        // Check if move is possible
        foreach(Cell cell in shape.cellList)
        {
            foreach (ICheckBlockedProperty blockedChecker in GetComponents<ICheckBlockedProperty>())
            {
                if (blockedChecker.IsBlocked(cell.GetGridPosition() + move))
                {
                    return;
                }
            }
        }

        // Pre-move per cell
        foreach (Cell cell in shape.cellList)
        {
            foreach (IPreTransformCellProperty pre in GetComponents<IPreTransformCellProperty>())
            {
                pre.PreTransform(cell, cell.GetGridPosition() + move);
            }
        }

        // Pre-move
        foreach (IPreTransformBlockProperty pre in GetComponents<IPreTransformBlockProperty>())
        {
            pre.PreTransform(move);
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
    }

    public void Translate(int x, int y)
    {
        Translate(new Vector2Int(x, y));
    }

    public void SetPosition(Vector2Int position)
    {
        Translate(position - shape.aabbCenter);
    }

    public void SetPosition(int x, int y)
    {
        SetPosition(new Vector2Int(x, y));
    }

    private void UpdatePositionUsingShapeAABB()
    {
        transform.position = shape.aabbCenterAbsolute + grid.unitLength * new Vector2(0.5f, 0.5f);
    }
    
    void Kill()
    {
        foreach(IOnKillProperty onKillProperty in GetComponents<IOnKillProperty>())
        {
            onKillProperty.OnKill();
        }
    }
}
