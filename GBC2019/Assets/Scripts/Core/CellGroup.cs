using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Contains a list of cells for each location 
// </summary>
public class CellGroup
{
    List<Cell> cells;

    public CellGroup()
    {
        cells = new List<Cell>();
    }

    public void AddCell(Cell cell)
    {
        cells.Add(cell);
    }

    public bool RemoveCell(Cell cell)
    {
        return cells.Remove(cell);
    }
}

