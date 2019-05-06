using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbMatchingSubtype : MonoBehaviour, IPreTransformCellProperty, IPreTransformBlockProperty, IResetProperty //, IPreTransformCellProperty to detect blocks, IPreTransformBlockProperty tomove these blocks in the same direction, IPostTransformBlockProperty to add these blocks
{
    public uint priority;

    private List<Block> blocksToAbsorb = new List<Block>();

    CellGroup group;

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
                    /*&& groupCell.GetParentBlock().GetComponent<AbsorbableByMatchingSubType>() 
                    && groupCell.GetParentBlock().GetComponent<AbsorbableByMatchingSubType>().priority <= priority*/)
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
            foreach (Block block in blocksToAbsorb)
            {
                GetComponent<Block>().Add(block);
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
