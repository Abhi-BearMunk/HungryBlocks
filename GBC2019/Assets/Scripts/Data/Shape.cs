using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// <summary>
// A data structure to hold shapes formed by Cells.
// </summary>
// <remarks>
// It maintains a 2D list of Cells corresponding to the bounding box defined by the shape.
// Any location within the bounding box not containing a Cell belonging to the shape is set to NULL.
// </remarks>
public class Shape
{
    // A 2D array of cells as a List of Cell Rows
    List<List<Cell>> cellMatrix = new List<List<Cell>>();

    // A shape has a AABB with the origin at the bottom left
    //
    // Primary parameters
    public int left { get; private set; }
    public int right { get; private set; }
    public int bottom { get; private set; }
    public int top { get; private set; }
    // Secondary parameters
    public Vector2Int bottomLeft { get; private set; }
    public Vector2Int topLeft { get; private set; }
    public Vector2Int bottomRight { get; private set; }
    public Vector2Int topRight { get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }
    public Vector2Int aabbCenter { get; private set; }
    public Vector2 aabbCenterAbsolute { get; private set; }
    // Boundaries (incl. corners)
    public List<Cell> leftBoundary { get; private set; }
    public List<Cell> rightBoundary { get; private set; }
    public List<Cell> bottomBoundary { get; private set; }
    public List<Cell> topBoundary { get; private set; }


    public Shape()
    {
        //// Just set the boundary values to extreme if no cells are provided during creation
        //left = 1000;
        //right = -1000;
        //bottom = 1000;
        //right = -1000;
    }

    public Shape(List<Cell> cells)
    {
        CreateShape(cells);
    }

    // <summary>
    // Create a shape from an unsorted list of cells on a grid
    // </summary>
    void CreateShape(List<Cell> cells)
    {
        // If no cells, nothing to do
        if(cells.Count == 0)
        {
            return;
        }

        #region Sort Cells
        // Start with some valid values for left and right
        left = cells[0].GetGridPosition().x;
        right = cells[0].GetGridPosition().x;

        // Sort cells first bottom to up and then left to right (within the same row)
        cells.Sort((Cell cell1, Cell cell2) =>
        {
            int columnComparision = cell1.GetGridPosition().x.CompareTo(cell2.GetGridPosition().x);
            int rowComparision = cell1.GetGridPosition().y.CompareTo(cell2.GetGridPosition().y);
            // Set left and right in case of extremities
            left = Mathf.Min(left, cell1.GetGridPosition().x, cell2.GetGridPosition().x);
            right = Mathf.Max(right, cell1.GetGridPosition().x, cell2.GetGridPosition().x);
            return rowComparision == 0 ? columnComparision : rowComparision;
        });

        // Set bottom and top using the first and last elements of the sorted list
        bottom = cells[0].GetGridPosition().y;
        top = cells[cells.Count - 1].GetGridPosition().y;
        #endregion

        #region Fill Matrix
        // A counter for the next element to add to the matrix, starting with the first
        int counter = 0;
        // A temp variable to store Cells during the upcoming loop
        Cell temp;

        // Loop through rows
        for (int i = bottom; i <= top; i++)
        {
            // Add a new row
            cellMatrix.Add(new List<Cell>());

            // Add a new entry to the left and right boundary
            leftBoundary.Add(null);
            rightBoundary.Add(null);

            // Loop through columns
            for (int j = left; j <= right; j++)
            {
                // Fill up the bottom and top boundaries during the first run through the columns
                if(bottomBoundary.Count < right - left + 1)
                {
                    bottomBoundary.Add(null);
                    topBoundary.Add(null);
                }

                // The first cell not yet added to the matrix
                temp = cells[counter];
                // If there is a position match with our current location the matrix
                if (temp.GetGridPosition().x == j && temp.GetGridPosition().y == i)
                {
                    // Add to the matrix and make counter point to the next Cell
                    cellMatrix[i].Add(temp);
                    counter++;

                    // Find the effect of the newly added Cell on the boundaries
                    //
                    // Left Boundary : If this is the first non-null on this row, then it must be the left boundary
                    if(leftBoundary[leftBoundary.Count - 1] == null)
                    {
                        leftBoundary[leftBoundary.Count - 1] = temp;
                    }
                    // Right Boundary : Keep setting the latest non-null on this row to be the right boundary
                    rightBoundary[rightBoundary.Count - 1] = temp;
                    // Bottom Boundary : If this is the first non-null on this column, then it must be the bottom boundary
                    if (bottomBoundary[j - left] == null)
                    {
                        bottomBoundary[j - left] = temp;
                    }
                    // Top Boundary : Keep setting the latest non-null on this column to be the top boundary
                    rightBoundary[j - left] = temp;

                    continue;
                }
                // If there was no match, just add null
                cellMatrix[i].Add(null);
            }
        }
        #endregion

        // Update AABB
        SetSecondaryParameters();
    }

    // <summary>
    // Add a new cell on the grid to this shape
    // </summary>
    public void AddCell(Cell cell)
    {
        // If the shape is empty, just intialize with it
        if(cellMatrix.Count == 0)
        {
            CreateShape(new List<Cell>() { cell });
            return;
        }

        #region Extend AABB
        // Get offsets from the boundary to check if new cell is out of AABB bounds
        int bottomDelta = cell.GetGridPosition().y - bottom;
        int topDelta = cell.GetGridPosition().y - top;
        int leftDelta = cell.GetGridPosition().x - left;
        int rightDelta = cell.GetGridPosition().x - right;

        bool bottomExtreme = false;
        bool topExtreme = false;
        bool leftExtreme = false;
        bool rightExtreme = false;

        // Extend the AABB as necessary
        while (bottomDelta < 0)
        {
            AddRowBottom();
            bottomDelta++;
            bottomExtreme = true;
        }
        while (topDelta > 0)
        {
            AddRowTop();
            topDelta--;
            topExtreme = true;
        }
        while (leftDelta < 0)
        {
            AddColumnLeft();
            leftDelta++;
            leftExtreme = true;
        }
        while (rightDelta > 0)
        {
            AddColumnRight();
            rightDelta--;
            rightExtreme = true;
        }
        #endregion

        // Add the cell to the matrix
        cellMatrix[cell.GetGridPosition().y - bottom][cell.GetGridPosition().x - left] = cell;

        #region Update Boundary
        // Update the boundaries as necessary, checking if the entry was newly added and hence null OR if it is a new cell over a existing one
        if (bottomBoundary[cell.GetGridPosition().x - left] == null || bottomExtreme)
        {
            bottomBoundary[cell.GetGridPosition().x - left] = cell;
        }
        if (topBoundary[cell.GetGridPosition().x - left] == null || topExtreme)
        {
            topBoundary[cell.GetGridPosition().x - left] = cell;
        }
        if (rightBoundary[cell.GetGridPosition().y - bottom] == null || rightExtreme)
        {
            rightBoundary[cell.GetGridPosition().y - bottom] = cell;
        }
        if (leftBoundary[cell.GetGridPosition().y - bottom] == null || leftExtreme)
        {
            leftBoundary[cell.GetGridPosition().y - bottom] = cell;
        }
        #endregion
    }

    void AddRowBottom()
    {
        // Create new row in the matrix
        cellMatrix.Insert(0, new List<Cell>());
        // Fill up the new row
        for (int i = left; i <= right; i++)
        {
            cellMatrix[0].Add(null);
        }
        // Extend the Orthogonal boundaries
        leftBoundary.Insert(0, null);
        rightBoundary.Insert(0, null);
        // Lower the bottom
        bottom--;
        // Update AABB
        SetSecondaryParameters();
    }

    void RemoveBottomRows(int count = 1)
    {
        cellMatrix.RemoveRange(0, count);

    }

    void AddRowTop()
    {
        // Create new entry in the matrix
        cellMatrix.Add(new List<Cell>());
        // Fill up the new row
        for (int i = left; i <= right; i++)
        {
            cellMatrix[cellMatrix.Count - 1].Add(null);
        }
        // Extend the Orthogonal boundaries
        leftBoundary.Add(null);
        rightBoundary.Add(null);
        // Raise the top
        top++;
        // Update AABB
        SetSecondaryParameters();
    }

    void AddColumnLeft()
    {
        // Create a new column entry on each row on the matrix
        foreach (List<Cell> row in cellMatrix)
        {
            row.Insert(0, null);
        }
        // Extend the Orthogonal boundaries
        bottomBoundary.Insert(0, null);
        topBoundary.Insert(0, null);
        // Pull left
        left--;
        // Update AABB
        SetSecondaryParameters();
    }

    void AddColumnRight()
    {
        // Create a new column entry on each row on the matrix
        foreach (List<Cell> row in cellMatrix)
        {
            row.Add(null);
        }
        // Extend the Orthogonal boundaries
        bottomBoundary.Add(null);
        topBoundary.Add(null);
        // Push right
        right++;
        // Update AABB
        SetSecondaryParameters();
    }

    public Cell RemoveCell(Vector2Int cellPos)
    {

        return null;
    }

    public Cell RemoveCell(Cell cell)
    {
        return RemoveCell(cell.GetGridPosition());
    }

    void SetSecondaryParameters()
    {
        bottomLeft = new Vector2Int(left, bottom);
        topLeft = new Vector2Int(left, top);
        bottomRight = new Vector2Int(right, bottom);
        topRight = new Vector2Int(right, top);
        width = right - left + 1;
        height = top - bottom + 1;
        aabbCenterAbsolute = new Vector2((left + right) * 0.5f, (top + bottom) * 0.5f);
        aabbCenter = new Vector2Int((int)Mathf.Ceil(aabbCenterAbsolute.x), (int)Mathf.Ceil(aabbCenterAbsolute.y));
    }
}
