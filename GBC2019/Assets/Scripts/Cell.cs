using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChangeCellTypeEvent : UnityEvent<Cell.CellType>
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
    private CellType type;
    public CellType cellType
    {
        get
        {
            return type;
        }
        set
        {
            type = cellType;
            OnChangeCellType.Invoke(cellType);
        }
    }
    public ChangeCellTypeEvent OnChangeCellType;

    private void Awake()
    {
        cellType = type;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {

    }
}
