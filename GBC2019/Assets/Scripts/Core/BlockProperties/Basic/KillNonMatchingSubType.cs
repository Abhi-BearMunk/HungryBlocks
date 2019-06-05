using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillNonMatchingSubType : MonoBehaviour, IPostTransformCellProperty, IPostTransformBlockProperty, IResetProperty, IRegisterProperty
{
    Block block;
    List<Cell> cellsToKill = new List<Cell>();

    public void Register(Block _block)
    {
        block = _block;
    }

    public void PostTransform(Cell cell)
    {
        CellGroup group = block.GetGrid().GetCellGroup(cell.GetGridPosition());
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
                if (groupCell != cell && groupCell.GetParentBlock().killableByNonMatchingSubType && groupCell.GetParentBlock().GetBlockSubType() != cell.GetParentBlock().GetBlockSubType())
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
        Cell.KillCellsAndMaintainConnecetdness(cellsToKill);
    }

    public void Reset()
    {
        cellsToKill.Clear();
    }
}
