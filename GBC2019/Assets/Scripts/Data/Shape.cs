using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A data structure to hold shapes formed by Cells.
/// </summary>
/// <remarks>
/// It maintains a 2D list of Cells corresponding to the bounding box defined by the shape.
/// Any location within the bounding box not containing a Cell belonging to the shape is set to NULL.
/// </remarks>
[System.Serializable]
public class Shape
{
    // A 2D array of cells as a List of Cell Rows
    public List<List<Cell>> cellMatrix { get; private set; }
    /// <summary>
    /// An unordered list of Cells
    /// </summary>
    [SerializeField]
    public List<Cell> cellList;

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
    public Vector2 centerOfMassAbsolute { get; private set; }   
    // Boundaries (incl. corners)
    public List<Cell> leftBoundary { get; private set; }
    public List<Cell> rightBoundary { get; private set; }
    public List<Cell> bottomBoundary { get; private set; }
    public List<Cell> topBoundary { get; private set; }
    // Callback for parameter updates
    public delegate void Callback();
    public Callback OnSetSecondaryParameters;
    public Callback OnTranslate;
    public Callback OnRotate;
    public Callback OnRemoveLastCell;
    public delegate void CellCallback(Cell cell);
    public CellCallback OnAddCell;
    // Special Cell/ Cells
    public Cell centerOfMassCell { get; private set; }
    public List<Vector2Int> emptyLocations { get; private set; }
    // Local variables
    List<Cell> open = new List<Cell>();
    List<Cell> closed = new List<Cell>();
    List<Cell> alreadyChecked = new List<Cell>();
    List<Cell> neighbours;

    public Shape()
    {
        InitializeLists(new List<Cell>());
    }

    public Shape(List<Cell> cells)
    {
        InitializeLists(cells);
        CreateShape(cells);
    }

    /// <summary>
    /// Initialize all the lists in this class
    /// </summary>
    /// <param name="cells"></param>
    void InitializeLists(List<Cell> cells)
    {
        leftBoundary = new List<Cell>();
        rightBoundary = new List<Cell>();
        bottomBoundary = new List<Cell>();
        topBoundary = new List<Cell>();
        cellList = new List<Cell>(cells);
        cellMatrix = new List<List<Cell>>();
        emptyLocations = new List<Vector2Int>();
    }

    /// <summary>
    /// Create a shape from an unsorted list of cells on a grid
    /// </summary>
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

        centerOfMassAbsolute = Vector2.zero;

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

                if(counter < cells.Count)
                {
                    // The first cell not yet added to the matrix
                    temp = cells[counter];
                    // If there is a position match with our current location the matrix
                    if (temp.GetGridPosition().x == j && temp.GetGridPosition().y == i)
                    {
                        // Add to the matrix and make counter point to the next Cell
                        cellMatrix[i - bottom].Add(temp);
                        counter++;
                        centerOfMassAbsolute += new Vector2(temp.GetGridPosition().x - left, temp.GetGridPosition().y - bottom);
                        // Find the effect of the newly added Cell on the boundaries
                        //
                        // Left Boundary : If this is the first non-null on this row, then it must be the left boundary
                        if (leftBoundary[leftBoundary.Count - 1] == null)
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
                        topBoundary[j - left] = temp;

                        continue;
                    }
                }

                
                // If there was no match or already assigned all cells, just add null
                cellMatrix[i - bottom].Add(null);
                emptyLocations.Add(new Vector2Int(j - left, i - bottom));
            }
        }
        #endregion
        centerOfMassAbsolute /= cells.Count;
        centerOfMassCell = cells[0];

        foreach(Cell cell in cells)
        {
            if(Vector2.SqrMagnitude((Vector2)(cell.GetGridPosition() - bottomLeft) - centerOfMassAbsolute) < Vector2.SqrMagnitude((Vector2)(centerOfMassCell.GetGridPosition() - bottomLeft) - centerOfMassAbsolute))
            {
                centerOfMassCell = cell;
            }
        }

        // Update AABB
        SetSecondaryParameters();
    }

    /// <summary>
    /// Clear all the lists and set references to null
    /// </summary>
    void Refresh()
    {
        leftBoundary.Clear();
        rightBoundary.Clear();
        bottomBoundary.Clear();
        topBoundary.Clear();
        cellMatrix.Clear();
        emptyLocations.Clear();
        centerOfMassCell = null;
    }

    /// <summary>
    /// Add a cell on the grid to this shape
    /// </summary>
    public void AddCell(Cell cell)
    {
        cellList.Add(cell);
        Refresh();
        CreateShape(cellList);
        if (OnAddCell != null)
        {
            OnAddCell(cell);
        }
    }

    /// <summary>
    /// Add a list of cells on the grid to this shape
    /// </summary>
    /// <param name="cells"></param>
    public void AddCells(List<Cell> cells)
    {
        foreach(Cell cell in cells)
        {
            cellList.Add(cell);
            if (OnAddCell != null)
            {
                OnAddCell(cell);
            }
        }
        Refresh();
        CreateShape(cellList);
    }

    /// <summary>
    /// Remove a Cell by reference
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Cell RemoveCell(Cell cell)
    {
        Refresh();
        cellList.Remove(cell);
        if (cellList.Count == 0 && OnRemoveLastCell != null)
        {
            OnRemoveLastCell();
        }
        else
        {
            CreateShape(cellList);
        }
        return cell;
        //return RemoveCell(cell.GetGridPosition());
    }

    /// <summary>
    /// Remove a Cell by reference
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public void RemoveCells(List<Cell> cells)
    {
        Refresh();
        foreach (Cell cell in cells)
        {
            cellList.Remove(cell);
        }
        if (cellList.Count == 0 && OnRemoveLastCell != null)
        {
            OnRemoveLastCell();
        }
        else
        {
            CreateShape(cellList);
        }
    }
    /// <summary>
    /// Move the shape by a certain amount
    /// </summary>
    /// <param name="move">the delta of the translation</param>
    public void Translate(Vector2Int move)
    {
        foreach (List<Cell> row in cellMatrix)
        {
            foreach (Cell cell in row)
            {
                if(cell != null)
                {
                    cell.SetGridPosition(move + cell.GetGridPosition());
                }
            }
        }
        left += move.x;
        right += move.x;
        top += move.y;
        bottom += move.y;
        SetSecondaryParameters();
        if(OnTranslate != null)
        {
            OnTranslate();
        }
    }

    /// <summary>
    /// Rotate the block in the given direction
    /// </summary>
    /// <param name="direction"> +1 for clockwise, -1 for anti-clockwise; by 90 degrees</param>
    public void Rotate(int direction)
    {
        if (direction == 0)
        {
            return;
        }
        Refresh();
        direction = (int)Mathf.Sign(direction);
        foreach (Cell cell in cellList)
        {
            if (cell != null)
            {
                Vector2Int delta = cell.GetGridPosition() - aabbCenter;
                Vector2Int newPos = aabbCenter + new Vector2Int(delta.y * direction, -delta.x * direction);
                cell.SetGridPosition(newPos);
            }
        }
        CreateShape(cellList);
        if (OnRotate != null)
        {
            OnRotate();
        }
    }

    private void SetSecondaryParameters()
    {
        bottomLeft = new Vector2Int(left, bottom);
        topLeft = new Vector2Int(left, top);
        bottomRight = new Vector2Int(right, bottom);
        topRight = new Vector2Int(right, top);
        width = right - left + 1;
        height = top - bottom + 1;
        aabbCenterAbsolute = new Vector2((left + right) * 0.5f, (top + bottom) * 0.5f);
        aabbCenter = new Vector2Int((int)Mathf.Ceil(aabbCenterAbsolute.x), (int)Mathf.Ceil(aabbCenterAbsolute.y));
        if(OnSetSecondaryParameters != null)
        {
            OnSetSecondaryParameters();
        }
    }

    /// <summary>
    /// The result of rotating a target point on the grid about the center of this shape
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Vector2Int RotateRelative(Vector2Int target, int direction)
    {
        direction = (int)Mathf.Sign(direction);
        Vector2Int delta = target - aabbCenter;
        return aabbCenter + new Vector2Int(delta.y * direction, -delta.x * direction);
    }

    /// <summary>
    /// Check if a grid position is out of bounds of this shape's AABB
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool OutOfBounds(Vector2Int pos)
    {
        return pos.x < left || pos.x > right || pos.y < bottom || pos.y > top;
    }

    /// <summary>
    /// Find the neighbouring Cells to a grid position which are within this shape
    /// </summary>
    /// <remarks>
    /// Currently only checks 4 directions
    /// </remarks>
    /// <param name="pos"></param>
    /// <returns></returns>
    List<Cell> NeighboursInShape(Vector2Int pos)
    {
        List<Cell> ret = new List<Cell>();
        Vector2Int[] directions = { new Vector2Int(-1,0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };
        foreach(Vector2Int dir in directions)
        {
            Vector2Int neighbourPos = pos + dir;
            if (!OutOfBounds(neighbourPos) && cellMatrix[neighbourPos.y - bottom][neighbourPos.x - left] != null)
            {
                ret.Add(cellMatrix[neighbourPos.y - bottom][neighbourPos.x - left]);
            }
        }

        return ret;
    }

    /// <summary>
    /// A list of all cells connected to the center of mass by a valid path
    /// </summary>
    /// <returns></returns>
    public List<Cell> ConnectedCells()
    {
        // If center of mass is null (or in other words cell list is empty),
        if(centerOfMassCell == null)
        {
            Debug.Log("Center of mass cell not valid");
            return new List<Cell>();
        }
        open.Clear();
        closed.Clear();
        alreadyChecked.Clear();
        // Start with the center of mass cell
        open.Add(centerOfMassCell);

        // Untill we run out of neghbouring cells...
        while (open.Count > 0)
        {
            // Pick the first cell from the open list
            Cell cell = open[0];
            // Check thge neighbours of this cell
            neighbours = NeighboursInShape(cell.GetGridPosition());
            foreach (Cell neighbour in neighbours)
            {
                // If the neighbour is in neither set, that means its untouched...
                if (!closed.Contains(neighbour) && !open.Contains(neighbour))
                {
                    // So put it in open
                    open.Add(neighbour);
                }
            }
            // We are done with this cell...
            // So remove it from open
            open.Remove(cell);
            // And put it in closed
            closed.Add(cell);
        }

        // Evethying in closed is reachable from the center of mass
        return closed;
    }
}

//public Cell RemoveCell(Vector2Int cellPos)
//{
//    if(cellList.Count == 0)
//    {
//        Debug.Log("Shape is Empty");
//        return null;
//    }

//    int row = cellPos.y - bottom;
//    int column = cellPos.x - left;
//    if(row < 0 || column < 0 || row >= cellMatrix.Count || column >= cellMatrix[row].Count)
//    {
//        Debug.Log("Remove Cell attempt not in AABB");
//        return null;
//    }
//    //if(cellMatrix[row][column] == null)
//    //{
//    //    Debug.Log("No Cell at Remove attempt location");
//    //    return null;
//    //}

//    Cell cell = cellMatrix[row][column];
//    cellMatrix[row][column] = null;



//    #region Collapse AABB and update boundaries
//    /*
//    #region Left
//    // If it is on the left Extreme
//    if (column == 0)
//    {
//        // Find a new entry for this row in the Left Boundary
//        for (int i = 0; i < cellMatrix[row].Count; i++)
//        {
//            leftBoundary[row] = cellMatrix[row][i];
//            if (leftBoundary[row] != null)
//            {
//                break;
//            }
//        }

//        // Find the current left most point
//        int leftMost = right;
//        for (int i = 0; i < leftBoundary.Count; i++)
//        {
//            if(leftBoundary[i] != null && leftBoundary[i].GetGridPosition().x < leftMost)
//            {
//                leftMost = leftBoundary[i].GetGridPosition().x;
//            }
//        }

//        // Collapse till left most if necessary
//        while(left < leftMost)
//        {
//            RemoveColumnLeft();
//        }
//    }
//    #endregion
//    #region Right
//    // If it is on the right Extreme
//    if (column == width)
//    {
//        // Find a new entry for this row in the Right Boundary
//        for (int i = cellMatrix[row].Count - 1; i >= 0; i--)
//        {
//            rightBoundary[row] = cellMatrix[row][i];
//            if (rightBoundary[row] != null)
//            {
//                break;
//            }
//        }

//        // Find the current right most point
//        int rightMost = left;
//        for (int i = 0; i < rightBoundary.Count; i++)
//        {
//            if (rightBoundary[i] != null && rightBoundary[i].GetGridPosition().x > rightMost)
//            {
//                rightMost = rightBoundary[i].GetGridPosition().x;
//            }
//        }

//        // Collapse till left most if necessary
//        while (right > rightMost)
//        {
//            RemoveColumnRight();
//        }
//    }
//    #endregion
//    // Recalculate in case columns got removed
//    row = cellPos.y - bottom;
//    column = cellPos.x - left;
//    #region Bottom
//    // If it is on the bottom Extreme
//    if (row == 0)
//    {
//        // Find a new entry for this column in the bottom Boundary
//        if (column >= 0 && column <= width)
//        {
//            for (int i = 0; i < cellMatrix.Count; i++)
//            {
//                bottomBoundary[column] = cellMatrix[i][column];
//                if (bottomBoundary[column] != null)
//                {
//                    break;
//                }
//            }
//        }

//        // Find the current bottom most point
//        int bottomMost = top;
//        for (int i = 0; i < bottomBoundary.Count; i++)
//        {
//            if (bottomBoundary[i] != null && bottomBoundary[i].GetGridPosition().y < bottomMost)
//            {
//                bottomMost = bottomBoundary[i].GetGridPosition().y;
//            }
//        }

//        // Collapse till bottom most if necessary
//        while (bottom < bottomMost)
//        {
//            RemoveRowBottom();
//        }
//    }
//    #endregion
//    #region Top
//    // If it is on the top Extreme
//    if (row == height)
//    {
//        // Find a new entry for this column in the top Boundary
//        if (column >= 0 && column <= width)
//        {
//            for (int i = cellMatrix.Count - 1; i >= 0; i--)
//            {
//                topBoundary[column] = cellMatrix[i][column];
//                if (topBoundary[column] != null)
//                {
//                    break;
//                }
//            }
//        }

//        // Find the current top most point
//        int topMost = bottom;
//        for (int i = 0; i < topBoundary.Count; i++)
//        {
//            if (topBoundary[i] != null && topBoundary[i].GetGridPosition().y > topMost)
//            {
//                topMost = topBoundary[i].GetGridPosition().y;
//            }
//        }

//        // Collapse till top most if necessary
//        while (top > topMost)
//        {
//            RemoveRowTop();
//        }
//    }
//    #endregion
//*/
//    #endregion
//    // Remove the cell from the cell list
//    cellList.Remove(cell);

//    // If this was the last one
//    if (cellList.Count == 0 && OnRemoveLastCell != null)
//    {
//        OnRemoveLastCell();
//    }
//    else
//    {
//        leftBoundary = new List<Cell>();
//        rightBoundary = new List<Cell>();
//        bottomBoundary = new List<Cell>();
//        topBoundary = new List<Cell>();

//        CreateShape(cellList);
//    }       
//    return cell;
//}

//public void AddCell(Cell cell)
//{
//    // If the shape is empty, just intialize with it
//    if(cellMatrix.Count == 0)
//    {
//        CreateShape(new List<Cell>() { cell });
//        return;
//    }

//    #region Extend AABB
//    // Get offsets from the boundary to check if new cell is out of AABB bounds
//    int bottomDelta = cell.GetGridPosition().y - bottom;
//    int topDelta = cell.GetGridPosition().y - top;
//    int leftDelta = cell.GetGridPosition().x - left;
//    int rightDelta = cell.GetGridPosition().x - right;

//    bool bottomExtreme = false;
//    bool topExtreme = false;
//    bool leftExtreme = false;
//    bool rightExtreme = false;

//    // Extend the AABB as necessary
//    while (bottomDelta < 0)
//    {
//        AddRowBottom();
//        bottomDelta++;
//        bottomExtreme = true;
//    }
//    while (topDelta > 0)
//    {
//        AddRowTop();
//        topDelta--;
//        topExtreme = true;
//    }
//    while (leftDelta < 0)
//    {
//        AddColumnLeft();
//        leftDelta++;
//        leftExtreme = true;
//    }
//    while (rightDelta > 0)
//    {
//        AddColumnRight();
//        rightDelta--;
//        rightExtreme = true;
//    }
//    #endregion

//    // Add the cell to the matrix
//    cellMatrix[cell.GetGridPosition().y - bottom][cell.GetGridPosition().x - left] = cell;
//    // Add the cell to the cell list
//    cellList.Add(cell);

//    #region Update Boundary
//    // Update the boundaries as necessary, checking if the entry was newly added and hence null OR if it is a new cell over a existing one
//    if (bottomBoundary[cell.GetGridPosition().x - left] == null || bottomExtreme)
//    {
//        bottomBoundary[cell.GetGridPosition().x - left] = cell;
//    }
//    if (topBoundary[cell.GetGridPosition().x - left] == null || topExtreme)
//    {
//        topBoundary[cell.GetGridPosition().x - left] = cell;
//    }
//    if (rightBoundary[cell.GetGridPosition().y - bottom] == null || rightExtreme)
//    {
//        rightBoundary[cell.GetGridPosition().y - bottom] = cell;
//    }
//    if (leftBoundary[cell.GetGridPosition().y - bottom] == null || leftExtreme)
//    {
//        leftBoundary[cell.GetGridPosition().y - bottom] = cell;
//    }
//    #endregion
//}
//void AddRowBottom()
//{
//    // Create new row in the matrix
//    cellMatrix.Insert(0, new List<Cell>());
//    // Fill up the new row
//    for (int i = left; i <= right; i++)
//    {
//        cellMatrix[0].Add(null);
//    }
//    // Extend the Orthogonal boundaries
//    leftBoundary.Insert(0, null);
//    rightBoundary.Insert(0, null);
//    // Lower the bottom
//    bottom--;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void AddRowTop()
//{
//    // Create new entry in the matrix
//    cellMatrix.Add(new List<Cell>());
//    // Fill up the new row
//    for (int i = left; i <= right; i++)
//    {
//        cellMatrix[cellMatrix.Count - 1].Add(null);
//    }
//    // Extend the Orthogonal boundaries
//    leftBoundary.Add(null);
//    rightBoundary.Add(null);
//    // Raise the top
//    top++;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void AddColumnLeft()
//{
//    // Create a new column entry on each row on the matrix
//    foreach (List<Cell> row in cellMatrix)
//    {
//        row.Insert(0, null);
//    }
//    // Extend the Orthogonal boundaries
//    bottomBoundary.Insert(0, null);
//    topBoundary.Insert(0, null);
//    // Pull left
//    left--;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void AddColumnRight()
//{
//    // Create a new column entry on each row on the matrix
//    foreach (List<Cell> row in cellMatrix)
//    {
//        row.Add(null);
//    }
//    // Extend the Orthogonal boundaries
//    bottomBoundary.Add(null);
//    topBoundary.Add(null);
//    // Push right
//    right++;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void RemoveRowBottom()
//{
//    if (cellMatrix.Count == 0)
//    {
//        Debug.Log("Can't remove bottom row, shape is empty");
//        return;
//    }
//    // Remove the bottom-most row from the matrix
//    cellMatrix.RemoveAt(0);
//    // Collapse the Orthogonal boundaries
//    leftBoundary.RemoveAt(0);
//    rightBoundary.RemoveAt(0);
//    // Raise the bottom
//    bottom++;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void RemoveRowTop()
//{
//    if (cellMatrix.Count == 0)
//    {
//        Debug.Log("Can't remove top row, shape is empty");
//        return;
//    }
//    // Remove the top-most row from the matrix
//    cellMatrix.RemoveAt(cellMatrix.Count - 1);
//    // Collapse the Orthogonal boundaries
//    leftBoundary.RemoveAt(leftBoundary.Count - 1);
//    rightBoundary.RemoveAt(rightBoundary.Count - 1);
//    // Lower the top
//    top--;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void RemoveColumnLeft()
//{
//    if (cellMatrix.Count == 0)
//    {
//        Debug.Log("Can't remove Left column, shape is empty");
//        return;
//    }
//    // Remove the left column entry on each row on the matrix
//    foreach (List<Cell> row in cellMatrix)
//    {
//        row.RemoveAt(0);
//    }
//    // Collapse the Orthogonal boundaries
//    bottomBoundary.RemoveAt(0);
//    topBoundary.RemoveAt(0);
//    // Push left
//    left++;
//    // Update AABB
//    SetSecondaryParameters();
//}

//void RemoveColumnRight()
//{
//    if (cellMatrix.Count == 0)
//    {
//        Debug.Log("Can't remove Right column, shape is empty");
//        return;
//    }
//    // Remove the right column entry on each row on the matrix
//    foreach (List<Cell> row in cellMatrix)
//    {
//        row.RemoveAt(row.Count - 1);
//    }
//    // Collapse the Orthogonal boundaries
//    bottomBoundary.RemoveAt(bottomBoundary.Count - 1);
//    topBoundary.RemoveAt(topBoundary.Count - 1);
//    // Pull right
//    right--;
//    // Update AABB
//    SetSecondaryParameters();
//}