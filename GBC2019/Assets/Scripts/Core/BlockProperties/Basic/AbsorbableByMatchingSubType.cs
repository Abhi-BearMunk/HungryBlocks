using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class AbsorbableByMatchingSubType : MonoBehaviour, IPreTransformCellProperty, IRegisterProperty 
{
    private Block block;
    private CellGroup group;

    public void Register(Block _block)
    {
        block = _block;
    }

    public bool PreTransform(Cell cell, Vector2Int pos)
    {
        group = cell.GetParentBlock().GetGrid().GetCellGroup(pos);
        if (group != null)
        {
            foreach (Cell groupCell in group.cells)
            {
                // Hack: Move destroy to late update
                if (groupCell.GetParentBlock() == null)
                {
                    Debug.Log("Group Cell parent block null");
                    continue;
                }
                if (groupCell.GetParentBlock() != cell.GetParentBlock() && groupCell.GetParentBlock().GetBlockSubType() == cell.GetParentBlock().GetBlockSubType()
                    && groupCell.GetParentBlock().absorbMatchingSubtype
                    && groupCell.GetParentBlock().absorbData
                    && groupCell.GetParentBlock().absorbData.priority >= block.absorbData.priority
                    && !block.absorbData.ignoreTypes.Contains(groupCell.GetParentBlock().absorbData.absorbType))
                {
                    groupCell.GetParentBlock().Add(block);
                    return false;
                }
            }
        }
        return true;
    }
}
