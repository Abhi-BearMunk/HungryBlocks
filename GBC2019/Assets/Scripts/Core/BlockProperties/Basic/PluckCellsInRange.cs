using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PluckCellsInRange : MonoBehaviour, IRegisterProperty
{
    public UnityEvent OnAbsorb;
    public int range = 5;

    Block block;
    List<Cell> absorb = new List<Cell>();
    public void Register(Block _block)
    {
        block = _block;
    }

    public void Absorb(Block.CellSubType subType)
    {
        Vector2Int pos;

        // Left Boundary
        foreach (Cell cell in block.GetShape().leftBoundary)
        {
            if(cell != null)
            {
                pos = cell.GetGridPosition();
                for (int i = 0; i < range; i++)
                {
                    pos.x--;
                    CheckGroup(pos, subType);
                }
            }          
        }

        // Right Boundary
        foreach (Cell cell in block.GetShape().rightBoundary)
        {
            if (cell != null)
            {
                pos = cell.GetGridPosition();
                for (int i = 0; i < range; i++)
                {
                    pos.x++;
                    CheckGroup(pos, subType);
                }
            }
        }

        // Top Boundary
        foreach (Cell cell in block.GetShape().topBoundary)
        {
            if (cell != null)
            {
                pos = cell.GetGridPosition();
                for (int i = 0; i < range; i++)
                {
                    pos.y++;
                    CheckGroup(pos, subType);
                }
            }
        }

        // Bottom Boundary
        foreach (Cell cell in block.GetShape().topBoundary)
        {
            if (cell != null)
            {
                pos = cell.GetGridPosition();
                for (int i = 0; i < range; i++)
                {
                    pos.y--;
                    CheckGroup(pos, subType);
                }
            }
        }

        Vector2Int bottomLeft = block.GetShape().bottomLeft + new Vector2Int(-1, -1);
        Vector2Int topLeft = block.GetShape().topLeft + new Vector2Int(-1, 1);
        Vector2Int bottomRight = block.GetShape().bottomRight + new Vector2Int(1, -1);
        Vector2Int topRight = block.GetShape().topRight + new Vector2Int(1, 1);


        for(int i = 0; i < range; i++)
        {
            for (int j = 0; j < range; j++)
            {
                // Bottom left corner
                pos = bottomLeft + new Vector2Int(-i, -j);
                CheckGroup(pos, subType);
                // Top left corner
                pos = topLeft + new Vector2Int(-i, j);
                CheckGroup(pos, subType);
                // Bottom right corner
                pos = bottomRight + new Vector2Int(i, -j);
                CheckGroup(pos, subType);
                // Top right corner
                pos = topRight + new Vector2Int(i, j);
                CheckGroup(pos, subType);
            }
        }

        foreach (Cell cell in absorb)
        {
            cell.SetGridPosition(block.GetShape().aabbCenter);
            cell.Kill();
            OnAbsorb.Invoke();
        }

        absorb.Clear();
    }

    void CheckGroup(Vector2Int pos, Block.CellSubType subType)
    {
        CellGroup group = block.GetGrid().GetCellGroup(pos);
        if (group != null)
        {
            foreach (Cell groupCell in group.cells)
            {
                if (groupCell.GetParentBlock() != null && groupCell.GetParentBlock() != block && groupCell.GetParentBlock().GetBlockSubType() == subType)
                {
                    absorb.Add(groupCell);
                }
            }
        }
    }
}
