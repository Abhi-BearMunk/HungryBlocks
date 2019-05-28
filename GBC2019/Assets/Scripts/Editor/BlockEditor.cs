using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{

    void OnSceneGUI()
    {
        Block block = target as Block;

            Vector3[] verts;

       

        // Draw Occupancy
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 10;

        if (block.GetGrid())
        {
            for (int column = block.GetShape().left; column <= block.GetShape().right; column++)
            {
                for (int row = block.GetShape().bottom; row <= block.GetShape().top; row++)
                {
                    CellGroup group = block.GetGrid().GetCellGroup(row, column);
                    if (block != null && group != null && group.cells != null)
                    {
                        verts = new Vector3[]
                        {
                        (column * Vector3.right + row * Vector3.up) * block.GetGrid().unitLength + block.GetGrid().origin,
                        (column * Vector3.right + (row + 1) * Vector3.up) * block.GetGrid().unitLength + block.GetGrid().origin,
                        ((column + 1) * Vector3.right + (row + 1) * Vector3.up) * block.GetGrid().unitLength + block.GetGrid().origin,
                        ((column + 1) * Vector3.right + row * Vector3.up) * block.GetGrid().unitLength + block.GetGrid().origin
                         };
                        Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0, 0, 0, 1));
                        Handles.Label(((group.id + 0.5f) * Vector3.right + (row + 0.75f) * Vector3.up) * block.GetGrid().unitLength + block.GetGrid().origin, group.cells.Count.ToString(), style);
                    }
                }
            }
        }

        // Draw Boundaries
        foreach (Cell cell in block.GetShape().leftBoundary)
        {
            if (cell != null)
            {
                verts = new Vector3[]
                {
                    cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up
                };
                Handles.DrawSolidRectangleWithOutline(verts, Color.blue * new Color(1, 1, 1, 0.7f), Color.blue);
            }
        }

        foreach (Cell cell in block.GetShape().topBoundary)
        {
            if (cell != null)
            {
                verts = new Vector3[]
                {
                    cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right
                };
                Handles.DrawSolidRectangleWithOutline(verts, Color.yellow * new Color(1, 1, 1, 0.7f), Color.yellow);
            }
        }

        foreach (Cell cell in block.GetShape().rightBoundary)
        {
            if (cell != null)
            {
                verts = new Vector3[]
                {
                    cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up,
                    cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up
                };
                Handles.DrawSolidRectangleWithOutline(verts, Color.red * new Color(1, 1, 1, 0.7f), Color.red);
            }
        }

        foreach (Cell cell in block.GetShape().bottomBoundary)
        {
            if (cell != null)
            {
                verts = new Vector3[]
                {
                    cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right,
                    cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right
                };
                Handles.DrawSolidRectangleWithOutline(verts, Color.green * new Color(1, 1, 1, 0.7f), Color.green);
            }
        }

        if (block.GetGrid())
        {
            Vector3 comPos1 = (Vector3)(block.GetShape().centerOfMassCell.GetGridPosition() + new Vector2(0.5f, 0.5f)) * block.GetGrid().unitLength + block.GetGrid().origin;
            Vector3 comPos2 = (Vector3)(block.GetShape().centerOfMassAbsolute + (Vector2)block.GetShape().bottomLeft + new Vector2(0.5f, 0.5f)) * block.GetGrid().unitLength + block.GetGrid().origin;
            verts = new Vector3[]
                        {
                        comPos1 + new Vector3(0.1f, 0.1f),
                         comPos1 + new Vector3(-0.1f, 0.1f),
                          comPos1 + new Vector3(-0.1f, -0.1f),
                           comPos1 + new Vector3(0.1f, -0.1f)
                         };
            Handles.DrawSolidRectangleWithOutline(verts, Color.black, Color.black);

            verts = new Vector3[]
                        {
                        comPos2 + new Vector3(0.1f, 0.1f),
                         comPos2 + new Vector3(-0.1f, 0.1f),
                          comPos2 + new Vector3(-0.1f, -0.1f),
                           comPos2 + new Vector3(0.1f, -0.1f)
                         };
            Handles.DrawSolidRectangleWithOutline(verts, Color.red, Color.red);


            foreach (Cell cell in block.GetShape().ConnectedCells())
            {
                if (cell != null)
                {
                    verts = new Vector3[]
                    {
                    cell.transform.position + new Vector3(1, 1) * (block.GetGrid().unitLength / 1.4f),
                    cell.transform.position + new Vector3(-1, 1) * (block.GetGrid().unitLength / 1.4f),
                    cell.transform.position + new Vector3(-1, -1) * (block.GetGrid().unitLength / 1.4f),
                    cell.transform.position + new Vector3(1, -1) * (block.GetGrid().unitLength / 1.4f)
                    };
                    Handles.DrawSolidRectangleWithOutline(verts, Color.cyan * new Color(1, 1, 1, 0.7f), Color.cyan);
                }
            }
        }
    }
}
