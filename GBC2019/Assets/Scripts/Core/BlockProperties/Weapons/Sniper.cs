using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class Sniper : MonoBehaviour, IRegisterProperty, IWeapon
{
    public GameObject laserEffectPrefab;
    Block myBlock;
    GridManager grid;

    List<Cell> cellsToKill = new List<Cell>();
    List<Block> blocksAffected = new List<Block>();

    public void Register(Block block)
    {
        myBlock = block;
    }
    public void Shoot(Vector2Int direction)
    {
        Vector2Int position = myBlock.GetShape().aabbCenter;
        CellGroup group = myBlock.GetGrid().GetCellGroup(position);
        LineRenderer laserEffect = Instantiate(laserEffectPrefab).GetComponent<LineRenderer>();
        laserEffect.SetPosition(0, myBlock.GetGrid().WorldPosition(position));
        while(group != null)
        {
            if (group.cells.Count > 0)
            {
                foreach (Cell groupCell in group.cells)
                {
                    // Hack: Move destroy to late update
                    if (groupCell.GetParentBlock() == null)
                    {
                        Debug.Log("Group Cell null");
                        continue;
                    }
                    if (groupCell.GetParentBlock().killableByNonMatchingSubType && groupCell.GetParentBlock() != myBlock)
                    {
                        if (groupCell != null && !cellsToKill.Contains(groupCell))
                        {
                            cellsToKill.Add(groupCell);
                        }
                    }
                }
            }
            position += direction;
            group = myBlock.GetGrid().GetCellGroup(position);
        }
        laserEffect.SetPosition(1, myBlock.GetGrid().WorldPosition(position));

        Cell.KillCellsAndMaintainConnecetdness(cellsToKill);
    }
}

