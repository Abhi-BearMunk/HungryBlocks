using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChangeCellTypeEvent : UnityEvent<Cell.CellType>
{
}

[System.Serializable]
public class Vector2IntEvent : UnityEvent<Vector2Int>
{
}

[System.Serializable]
public class GridEvent : UnityEvent<GridManager>
{
}

// <summary>
// A unit which can be placed on the Grid.
// Generally exists as part of a Block.
// </summary>
// <remarks>
// Multiple cells can exist in the same position on the grid.
// </remarks>
public class Cell : MonoBehaviour, IPoolable
{
    public enum CellType { R, G, B, Y };
    [SerializeField]
    private CellType cellType;
    public ChangeCellTypeEvent OnChangeCellType;

    private GridManager grid;
    private Vector2Int gridPosition;
    public GridEvent OnSetGrid;
    public Vector2IntEvent OnSetPosition;

    private Block parentBlock = null;
    public BlockEvent OnSetParent;
    public BlockEvent OnChangeParent;

    public UnityEvent OnKill;
    private void Awake()
    {
        SetCellType(cellType);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCellType(CellType _cellType)
    {
        if(cellType != _cellType)
        {
            cellType = _cellType;
            OnChangeCellType.Invoke(cellType);
        }       
    }

    public CellType GetCellType()
    {
        return cellType;
    }

    public void SetGrid(GridManager gridManager)
    {
        grid = gridManager;
        OnSetGrid.Invoke(grid);
    }

    public void SetGridPosition(Vector2Int position)
    {
        grid.TryChangeCellGroup(this, position);
        gridPosition = position;
        OnSetPosition.Invoke(position);
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void SetParentBlock(Block block)
    {
        bool spawning = false;
        if(parentBlock == null)
        {
            spawning = true;
        }
        else
        {
            parentBlock.shape.RemoveCell(this);
        }
        parentBlock = block;
        SetCellType(parentBlock.GetBlockType());
        parentBlock.shape.AddCell(this);
        if(spawning)
        {
            OnSetParent.Invoke(parentBlock);
        }
        else
        {
            OnChangeParent.Invoke(parentBlock);
        }
    }

    public void Reset()
    {

    }

    public void Kill()
    {
        OnKill.Invoke();
        //TODO: Change it to pool
        Destroy(this.gameObject);
    }
}
