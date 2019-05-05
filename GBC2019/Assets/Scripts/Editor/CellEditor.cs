//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(Cell))]
//public class CellEditor : Editor
//{
//    //[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
//    //static void DrawHandles(Cell cell, GizmoType gizmoType)
//    //{
//    //    GUIStyle style = new GUIStyle();
//    //    style.alignment = TextAnchor.MiddleCenter;

//    //    int row = cell.GetGridPosition().y;
//    //    int column = cell.GetGridPosition().x;
//    //    GridManager grid = cell.GetParentBlock().GetGrid();
//    //    CellGroup group = cell.GetParentBlock().GetGrid().GetCellGroup(row, column);
//    //    if (group != null && group.cells.Count > 0)
//    //    {
//    //        Vector3[] verts = new Vector3[]
//    //        {
//    //                        (column * Vector3.right + row * Vector3.up) * grid.unitLength + grid.origin,
//    //                        (column * Vector3.right + (row + 1) * Vector3.up) * grid.unitLength + grid.origin,
//    //                        ((column + 1) * Vector3.right + (row + 1) * Vector3.up) * grid.unitLength + grid.origin,
//    //                        ((column + 1) * Vector3.right + row * Vector3.up) * grid.unitLength + grid.origin
//    //         };
//    //        Handles.DrawSolidRectangleWithOutline(verts, new Color(0.5f, 0.5f, 0.5f, 0.9f), new Color(0, 0, 0, 1));
//    //        Handles.Label(((column + 0.5f) * Vector3.right + (row + 1.0f) * Vector3.up) * grid.unitLength + grid.origin, group.cells.Count.ToString(), style);
//    //    }
//    //}
//}
