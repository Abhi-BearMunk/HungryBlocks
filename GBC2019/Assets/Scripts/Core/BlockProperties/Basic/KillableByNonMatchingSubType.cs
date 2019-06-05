using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillableByNonMatchingSubType : MonoBehaviour, IPostTransformCellProperty, IPostTransformBlockProperty, IResetProperty, IRegisterProperty
{
    Block block;
    List<Cell> cellsToKill = new List<Cell>();
    CellGroup group;

    public void Register(Block _block)
    {
        block = _block;
    }

    public void PostTransform(Cell cell)
    {
        group = block.GetGrid().GetCellGroup(cell.GetGridPosition());
        if (group != null && group.cells.Count > 1)
        {
            foreach (Cell groupCell in group.cells)
            {
                // Hack: Move destroy to late update
                if(groupCell.GetParentBlock() == null)
                {
                    Debug.Log("Group Cell null");
                    continue;
                }
                if (groupCell != cell && groupCell.GetParentBlock().killNonMatchingSubType && groupCell.GetParentBlock().GetBlockSubType() != cell.GetParentBlock().GetBlockSubType())
                {
                    if (!cellsToKill.Contains(cell))
                    {
                        cellsToKill.Add(cell);
                    }
                }
            }
        }
    }

    public void PostTransform()
    {
        foreach (Cell cell in cellsToKill)
        {
            if (cell != null)
            {
                cell.Kill();
            }
        }
        cellsToKill.Clear();


        block.KillDisconnectedCells();
    }

    public void Reset()
    {
        cellsToKill.Clear();
    }

}
