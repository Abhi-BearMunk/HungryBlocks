using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Contains a list of CellGroups
// </summary>
public class CellGroupRow
{
    int id;
    List<CellGroup> cells;

    public CellGroupRow(int _length, int _id = 0)
    {
        id = _id;
        cells = new List<CellGroup>();
        for (int i = 0; i < _length; i++)
        {
            cells.Add(new CellGroup());
        }
    }

    public CellGroup GetCellGroup(int column)
    {
        if (column >= cells.Count)
        {
            Debug.LogError("Column " + column + " does not exist on Row " + id);
            return null;
        }
        return cells[column];
    }

    public void AddCell(int column, Cell cell)
    {
        if(cell == null)
        {
            Debug.LogWarning("Cannot add null cell at grid " + id + " , " + column);
        }

        while (column >= cells.Count)
        {
            cells.Add(new CellGroup());
            Debug.LogWarning("Row " + id + " length insufficient for adding column " + column + ". Resizing!");
        }

        cells[column].AddCell(cell);
    }

    public bool RemoveCell(int column, Cell cell)
    {
        if (column >= cells.Count)
        {
            Debug.LogError("Column " + column + "does not exist on Row " + id);
            return false;
        }

        return cells[column].RemoveCell(cell);
    }
}
