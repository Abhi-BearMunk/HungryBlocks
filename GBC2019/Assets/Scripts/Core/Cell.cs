using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A unit which can be placed on the Grid.
/// Generally exists as part of a Block.
/// </summary>
/// <remarks>
/// Multiple cells can exist in the same position on the grid.
/// </remarks>
public class Cell : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridPosition;
    [SerializeField]
    private Block parentBlock = null;

    public UnityEvent OnPlacedOnGrid;
    public UnityEvent OnSetPosition;
    public UnityEvent OnSetPositionImmidiate;
    public UnityEvent OnSetParent;
    public UnityEvent OnKill;

    private void Awake()
    {
        if (GameObject.Find("Cells"))
        {
            transform.parent = GameObject.Find("Cells").transform;
        }
    }

    public void PlaceOnGrid()
    {
        OnPlacedOnGrid.Invoke();
    }

    public void SetGridPosition(Vector2Int position)
    {
        parentBlock.GetGrid().MoveCell(this, position);
        gridPosition = position;
        OnSetPosition.Invoke();
    }

    public void SetGridPositionImmidiate(Vector2Int position)
    {
        parentBlock.GetGrid().MoveCell(this, position);
        gridPosition = position;
        OnSetPositionImmidiate.Invoke();
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void SetParentBlock(Block block)
    {
        parentBlock = block;
        OnSetParent.Invoke();
    }

    public Block GetParentBlock()
    {
        return parentBlock;
    }

    /// <summary>
    /// Kill a cell removing it form the grid and its parent block
    /// </summary>
    public void Kill()
    {
        parentBlock.GetGrid().RemoveCell(gridPosition, this);
        parentBlock.GetShape().RemoveCell(this);
        OnKill.Invoke();
        Destroy(this);
    }

    /// <summary>
    /// Kill a list of cells and then make sure all the affected blocks stay connected
    /// </summary>
    /// <param name="cellsToKill"> list of cells to kill</param>
    public static void KillCellsAndMaintainConnecetdness(List<Cell> cellsToKill)
    {
        List<Block> blocksAffected = new List<Block>();

        foreach (Cell cell in cellsToKill)
        {
            if (cell != null)
            {
                if (!blocksAffected.Contains(cell.GetParentBlock()))
                {
                    blocksAffected.Add(cell.GetParentBlock());
                }
                cell.Kill();
            }
        }

        foreach (Block block in blocksAffected)
        {
            block.KillDisconnectedCells();
        }
    }
}
