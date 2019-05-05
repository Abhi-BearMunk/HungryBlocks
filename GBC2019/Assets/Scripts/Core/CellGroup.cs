using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Contains a list of cells for each location 
// </summary>
public class CellGroup
{
    public int id { get; private set; }
    public List<Cell> cells { get; private set; }

    public CellGroup(int _id = 0)
    {
        id = _id;
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

