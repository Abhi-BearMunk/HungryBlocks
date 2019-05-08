using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class AbsorbableByMatchingSubType : MonoBehaviour, IPreTransformCellProperty // IBlockeProperty to prevent moving if absorbable but we are SURE that it is not the one doing the absorbing
{
    private CellGroup group;

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
                    && groupCell.GetParentBlock().GetComponent<AbsorbMatchingSubtype>()
                    && groupCell.GetParentBlock().GetComponent<AbsorbData>()
                    && groupCell.GetParentBlock().GetComponent<AbsorbData>().priority >= GetComponent<AbsorbData>().priority
                    && !GetComponent<AbsorbData>().ignoreTypes.Contains(groupCell.GetParentBlock().GetComponent<AbsorbData>().absorbType))
                {
                    groupCell.GetParentBlock().Add(GetComponent<Block>());
                    return false;
                }
            }
        }
        return true;
    }
}
