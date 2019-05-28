using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class AbsorbMatchingSubtype : MonoBehaviour, IPreTransformCellProperty, IPreTransformBlockProperty, IResetProperty, IRegisterProperty 
{
    private Block block;
    private List<Block> blocksToAbsorb = new List<Block>();
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
                    && groupCell.GetParentBlock().absorbableByMatchingSubType
                    && groupCell.GetParentBlock().absorbData
                    && groupCell.GetParentBlock().absorbData.priority <= block.absorbData.priority
                    && !block.absorbData.ignoreTypes.Contains(groupCell.GetParentBlock().absorbData.absorbType))
                {
                    if(!blocksToAbsorb.Contains(groupCell.GetParentBlock()))
                    {
                        blocksToAbsorb.Add(groupCell.GetParentBlock());
                    }
                }
            }
        }
        return true;
    }

    public bool PreTransform(Vector2Int move)
    {
        if(blocksToAbsorb.Count > 0)
        {
            foreach (Block other in blocksToAbsorb)
            {
                block.Add(other);
            }
            return false;
        }
        return true;
    }

    public void Reset()
    {
        blocksToAbsorb.Clear();
    }
}
