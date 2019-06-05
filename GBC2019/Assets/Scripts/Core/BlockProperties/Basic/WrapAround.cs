using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This should be in the Block class not a property
public class WrapAround : MonoBehaviour, IPostTransformCellProperty
{
    public void PostTransform(Cell cell)
    {
        if(cell.GetGridPosition().x >= cell.GetParentBlock().GetGrid().GetWidth() || cell.GetGridPosition().y >= cell.GetParentBlock().GetGrid().GetHeight()
            || cell.GetGridPosition().x < 0 || cell.GetGridPosition().y < 0)
        {
            Vector2Int newPos = new Vector2Int((cell.GetGridPosition().x + cell.GetParentBlock().GetGrid().GetWidth()) % cell.GetParentBlock().GetGrid().GetWidth(), (cell.GetGridPosition().y + cell.GetParentBlock().GetGrid().GetHeight()) % cell.GetParentBlock().GetGrid().GetHeight());
            //cell.GetParentBlock().GetShape().RemoveCell(cell);
            cell.SetGridPositionImmidiate(newPos);
            //cell.GetParentBlock().GetShape().AddCell(cell);
        }
    }
}
