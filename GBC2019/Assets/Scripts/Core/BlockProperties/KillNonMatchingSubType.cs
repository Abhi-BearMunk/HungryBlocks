using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillNonMatchingSubType : MonoBehaviour, IPostTransformCellProperty, IPostTransformBlockProperty, IResetProperty
{

    List<Cell> cellsToKill = new List<Cell>();

    public void PostTransform(Cell cell)
    {
        CellGroup group = GetComponent<Block>().GetGrid().GetCellGroup(cell.GetGridPosition());
        if(group != null && group.cells.Count > 1)
        {
            foreach(Cell groupCell in group.cells)
            {
                // Hack: Move destroy to late update
                if (groupCell.GetParentBlock() == null)
                {
                    Debug.Log("Group Cell null");
                    continue;
                }
                if (groupCell != cell && groupCell.GetParentBlock().GetComponent<KillableByNonMatchingSubType>() && groupCell.GetParentBlock().GetBlockSubType() != cell.GetParentBlock().GetBlockSubType())
                {
                    if(!cellsToKill.Contains(groupCell))
                    {
                        cellsToKill.Add(groupCell);
                    }
                }
            }
        }
    }

    public void PostTransform()
    {
        foreach(Cell cell in cellsToKill)
        {
            if(cell != null)
            {
                cell.Kill();
            }
        }
        cellsToKill.Clear();
    }

    public void Reset()
    {
        cellsToKill.Clear();
    }
}
