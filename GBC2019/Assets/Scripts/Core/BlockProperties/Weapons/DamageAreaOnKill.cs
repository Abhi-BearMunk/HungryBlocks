using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAreaOnKill : MonoBehaviour, IOnKillProperty, IRegisterProperty
{
    public int halfWidth = 5;
    public int halfHeight = 5;
    public GameObject explosionEffectPrefab;

    List<Cell> cellsToKill = new List<Cell>();

    Block block;
    public void Register(Block _block)
    {
        block = _block;
    }

    public void OnKill()
    {
        Vector2Int blockCenter = block.GetShape().aabbCenter;
        CellGroup group = null;
        Instantiate(explosionEffectPrefab, block.GetGrid().origin + (new Vector3(blockCenter.x, blockCenter.y, 0) + new Vector3(0.5f, 0.5f, 0)) * block.GetGrid().unitLength, Quaternion.identity).transform.localScale = new Vector3((2 * halfWidth + 1) / 9.0f, (2 * halfHeight + 1) / 9.0f, 1);

        for (int row = blockCenter.y - halfHeight; row <= blockCenter.y + halfHeight; row++)
        {
            for (int column = blockCenter.x - halfWidth; column <= blockCenter.x + halfWidth; column++)
            {
                //Instantiate(explosionEffectPrefab, block.GetGrid().origin + (new Vector3(column, row, 0) + new Vector3(0.5f, 0.5f, 0)) * block.GetGrid().unitLength, Quaternion.identity);
                group = block.GetGrid().GetCellGroup(row, column);
                if (group != null && group.cells.Count > 0)
                {
                    foreach (Cell groupCell in group.cells)
                    {
                        // Hack: Move destroy to late update
                        if (groupCell.GetParentBlock() == null)
                        {
                            Debug.Log("Group Cell null");
                            continue;
                        }
                        if (groupCell.GetParentBlock().killableByNonMatchingSubType && groupCell.GetParentBlock().GetBlockSubType() != block.GetBlockSubType())
                        {
                            if(groupCell != null && !cellsToKill.Contains(groupCell))
                            {
                                cellsToKill.Add(groupCell);
                            }
                        }
                    }
                }
            }
        }

        Cell.KillCellsAndMaintainConnecetdness(cellsToKill);
    }
}
