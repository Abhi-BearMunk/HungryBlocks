using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanCollectPowerUp : MonoBehaviour, IPostTransformBlockProperty
{
    public void PostTransform()
    {
        List<Vector2Int> positionsToCheck = new List<Vector2Int>();
        Block block = GetComponent<Block>();

        foreach(Cell cell in block.GetShape().leftBoundary)
        {
            if(cell != null)
            {
                positionsToCheck.Add(cell.GetGridPosition() + new Vector2Int(-1, 0));
            }
        }

        foreach (Cell cell in block.GetShape().rightBoundary)
        {
            if (cell != null)
            {
                positionsToCheck.Add(cell.GetGridPosition() + new Vector2Int(1, 0));
            }
        }

        foreach (Cell cell in block.GetShape().topBoundary)
        {
            if (cell != null)
            {
                positionsToCheck.Add(cell.GetGridPosition() + new Vector2Int(0, 1));
            }
        }

        foreach (Cell cell in block.GetShape().bottomBoundary)
        {
            if (cell != null)
            {
                positionsToCheck.Add(cell.GetGridPosition() + new Vector2Int(0, -1));
            }
        }

        positionsToCheck.Add(block.GetShape().bottomLeft + new Vector2Int(-1, -1));
        positionsToCheck.Add(block.GetShape().bottomRight + new Vector2Int(1, -1));
        positionsToCheck.Add(block.GetShape().topRight + new Vector2Int(1, 1));
        positionsToCheck.Add(block.GetShape().topLeft + new Vector2Int(-1, 1));

        List<PowerUp> powerupsToConsume = new List<PowerUp>();
        CellGroup group;
        foreach(Vector2Int pos in positionsToCheck)
        {
            group = block.GetGrid().GetCellGroup(pos);
            if(group != null)
            {
                foreach(Cell cell in group.cells)
                {
                    if(cell.GetParentBlock() != null 
                        && cell.GetParentBlock().GetComponent<PowerUp>() 
                        && cell.GetParentBlock().GetComponent<PowerUp>().PerfectFit(block)
                        && !powerupsToConsume.Contains(cell.GetParentBlock().GetComponent<PowerUp>()))
                    {
                        powerupsToConsume.Add(cell.GetParentBlock().GetComponent<PowerUp>());
                    }
                }
            }
        }

        foreach(PowerUp p in powerupsToConsume)
        {
            p.ConsumePowerup(block);
        }
    }
}
