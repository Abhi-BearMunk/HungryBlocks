using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillNonMatchingSubType : MonoBehaviour, IPostTransformCellProperty, IPostTransformBlockProperty, IResetProperty, IRegisterProperty
{
    Block block;
    List<Cell> cellsToKill = new List<Cell>();
    List<Block> blocksAffected = new List<Block>();

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
        blocksAffected.Clear();
        foreach(Cell cell in cellsToKill)
        {
            if(cell != null)
            {
                if (!blocksAffected.Contains(cell.GetParentBlock()))
                {
                    blocksAffected.Add(cell.GetParentBlock());
                }
                cell.Kill();
            }
        }
        cellsToKill.Clear();

        foreach (Block block in blocksAffected)
        {
            block.KillDisconnectedCells();
        }
        blocksAffected.Clear();
    }

    public void Reset()
    {
        cellsToKill.Clear();
        blocksAffected.Clear();
    }
}
