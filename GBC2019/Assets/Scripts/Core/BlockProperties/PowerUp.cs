using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour, IPostTransformBlockProperty
{
    public System.Type weaponType;
    public void PostTransform()
    {
        Block block = GetComponent<Block>();
        if (block.GetShape().emptyLocations.Count == 0)
        {
            return;
        }
        Block other = null;
        CellGroup group = block.GetGrid().GetCellGroup(block.GetShape().emptyLocations[0] + block.GetShape().bottomLeft);
        if(group != null)
        {
            foreach(Cell cell in group.cells)
            {
                if(cell.GetParentBlock() != block && cell.GetParentBlock() != null && cell.GetParentBlock().GetComponent<CanCollectPowerUp>() )
                {
                    other = cell.GetParentBlock();
                    break;
                }
            }
        }

        if(other != null && PerfectFit(other))
        {        
            ConsumePowerup(other);
        }
    }

    public void ConsumePowerup(Block other)
    {
        if (other.GetComponent<WeaponHandler>())
        {
            other.GetComponent<WeaponHandler>().SetNewWeapon(weaponType);
        }
        List<Cell> cells = new List<Cell>(GetComponent<Block>().GetShape().cellList);
        foreach(Cell cell in cells)
        {
            cell.Kill();
        }
    }

    public bool PerfectFit(Block other)
    {
        Block block = GetComponent<Block>();
        CellGroup group;

        for(int i = 0; i < block.GetShape().cellMatrix.Count; i++)
        {
            for(int j = 0; j < block.GetShape().cellMatrix[i].Count; j++)
            {
                if(block.GetShape().cellMatrix[i][j] == null)
                {
                    group = block.GetGrid().GetCellGroup(new Vector2Int(j, i) + block.GetShape().bottomLeft);
                    if(group != null)
                    {
                        bool otherHasACellHere = false;
                        foreach(Cell cell in group.cells)
                        {
                            if(cell.GetParentBlock() != null && cell.GetParentBlock() == other)
                            {
                                otherHasACellHere = true;
                                break;
                            }
                        }
                        if(!otherHasACellHere)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    group = block.GetGrid().GetCellGroup(new Vector2Int(j, i) + block.GetShape().bottomLeft);
                    if (group != null)
                    {
                        bool otherHasACellHere = false;
                        foreach (Cell cell in group.cells)
                        {
                            if (cell.GetParentBlock() != null && cell.GetParentBlock() == other)
                            {
                                otherHasACellHere = true;
                                break;
                            }
                        }
                        if (otherHasACellHere)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }
}
