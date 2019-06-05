using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    bool showBoundaries;
    bool showCellOccupancy;

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    static void DrawHandles(GridManager grid, GizmoType gizmoType)
    {
        Handles.color = Color.cyan;
        for (int i = 0; i <= grid.GetHeight(); i++)
        {
            Handles.DrawLine(grid.origin + i * grid.unitLength * Vector3.up, grid.origin + i * grid.unitLength * Vector3.up + grid.unitLength * grid.GetWidth() * Vector3.right);
        }

        for (int i = 0; i <= grid.GetWidth(); i++)
        {
            Handles.DrawLine(grid.origin + i * grid.unitLength * Vector3.right, grid.origin + i * grid.unitLength * Vector3.right + grid.unitLength * grid.GetHeight() * Vector3.up);
        }
    }

    //[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
    //static void DrawHandles(GridManager grid, GizmoType gizmoType)
    //{
    //    Vector3[] verts;
    //    GUIStyle style = new GUIStyle();
    //    //style.fixedWidth = grid.unitLength;
    //    //style.fixedHeight = grid.unitLength;
    //    style.alignment = TextAnchor.MiddleCenter;


    //    if (showCellOccupancy && grid.grid != null)
    //    {
    //        foreach (CellGroupRow row in grid.grid)
    //        {
    //            foreach (CellGroup group in row.cells)
    //            {
    //                if (group.cells.Count > 0)
    //                {
    //                    verts = new Vector3[]
    //                    {
    //                    (group.id * Vector3.right + row.id * Vector3.up) * grid.unitLength + grid.origin,
    //                    (group.id * Vector3.right + (row.id + 1) * Vector3.up) * grid.unitLength + grid.origin,
    //                    ((group.id + 1) * Vector3.right + (row.id + 1) * Vector3.up) * grid.unitLength + grid.origin,
    //                    ((group.id + 1) * Vector3.right + row.id * Vector3.up) * grid.unitLength + grid.origin
    //                     };
    //                    Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.5f), new Color(0, 0, 0, 1));

    //                    Handles.Label(((group.id + 0.5f) * Vector3.right + (row.id + 1.0f) * Vector3.up) * grid.unitLength + grid.origin, group.cells.Count.ToString(), style);
    //                }
    //            }
    //        }
    //    }

    //    if(showBoundaries)
    //    {
    //        foreach(Block block in grid.blocks)
    //        {

    //            foreach (Cell cell in block.GetShape().leftBoundary)
    //            {
    //                verts = new Vector3[]
    //                {
    //                        cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up
    //                };
    //                Handles.DrawSolidRectangleWithOutline(verts, Color.blue * new Color(1, 1, 1, 0.7f), Color.blue);
    //            }

    //            foreach (Cell cell in block.GetShape().topBoundary)
    //            {
    //                verts = new Vector3[]
    //                {
    //                        cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right
    //                };
    //                Handles.DrawSolidRectangleWithOutline(verts, Color.yellow * new Color(1, 1, 1, 0.7f), Color.yellow);
    //            }

    //            foreach (Cell cell in block.GetShape().rightBoundary)
    //            {
    //                verts = new Vector3[]
    //                {
    //                        cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.right + block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position + 3 * block.GetGrid().unitLength / 4 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up,
    //                        cell.transform.position + block.GetGrid().unitLength / 2 * Vector3.right - block.GetGrid().unitLength / 2 * Vector3.up
    //                };
    //                Handles.DrawSolidRectangleWithOutline(verts, Color.red * new Color(1, 1, 1, 0.7f), Color.red);
    //            }

    //            foreach (Cell cell in block.GetShape().bottomBoundary)
    //            {
    //                verts = new Vector3[]
    //                {
    //                        cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.up + block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position - 3 * block.GetGrid().unitLength / 4 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right,
    //                        cell.transform.position - block.GetGrid().unitLength / 2 * Vector3.up - block.GetGrid().unitLength / 2 * Vector3.right
    //                };
    //                Handles.DrawSolidRectangleWithOutline(verts, Color.green * new Color(1, 1, 1, 0.7f), Color.green);
    //            }
    //        }

    //    }
    //}

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button((showBoundaries) ? "Hide Boundaries" : "Show Boundaries"))
        {
            showBoundaries = !showBoundaries;
            EditorPrefs.SetBool("ShowBoundaries", showBoundaries);
        }
        if (GUILayout.Button((showCellOccupancy) ? "Hide Cell Occupancy" : "Show Cell Occupancy"))
        {
            showCellOccupancy = !showCellOccupancy;
            EditorPrefs.SetBool("ShowCellOccupancy", showCellOccupancy);
        }
    }

    void OnEnable()
    {
        showBoundaries = EditorPrefs.GetBool("ShowBoundaries", false);
        showCellOccupancy = EditorPrefs.GetBool("ShowCellOccupancy", false);
    }

    void OnSceneGUI()
    {
        GridManager grid = target as GridManager;

        Vector3[] verts;
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;


        if (showCellOccupancy && grid.grid != null)
        {
            foreach (CellGroupRow row in grid.grid)
            {
                foreach (CellGroup group in row.cells)
                {
                    if (group.cells.Count > 0)
                    {
                        verts = new Vector3[]
                        {
                            (group.id * Vector3.right + row.id * Vector3.up) * grid.unitLength + grid.origin,
                            (group.id * Vector3.right + (row.id + 1) * Vector3.up) * grid.unitLength + grid.origin,
                            ((group.id + 1) * Vector3.right + (row.id + 1) * Vector3.up) * grid.unitLength + grid.origin,
                            ((group.id + 1) * Vector3.right + row.id * Vector3.up) * grid.unitLength + grid.origin
                         };
                        Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.9f), new Color(0, 0, 0, 1));
                        Handles.Label(((group.id + 0.5f) * Vector3.right + (row.id + 1.0f) * Vector3.up) * grid.unitLength + grid.origin, group.cells.Count.ToString(), style);
                    }
                }
            }
        }

        if (showBoundaries)
        {
            foreach (Block block in grid.blocks)
            {

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
            }

        }

       
    }
}
