using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    void OnSceneGUI()
    {
        LevelManager level = target as LevelManager;
        Vector3[] verts;
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        foreach (RectInt rectInt in level.safeSpawnRegions)
        {
            verts = new Vector3[]
                        {
                            (rectInt.xMin * Vector3.right + rectInt.yMin * Vector3.up) * level.grid.unitLength + level.grid.origin,
                            (rectInt.xMin * Vector3.right + (rectInt.yMin + rectInt.height) * Vector3.up) * level.grid.unitLength + level.grid.origin,
                            ((rectInt.xMin + rectInt.width) * Vector3.right + (rectInt.yMin + rectInt.height) * Vector3.up) * level.grid.unitLength + level.grid.origin,
                            ((rectInt.xMin + rectInt.width) * Vector3.right + rectInt.yMin * Vector3.up) * level.grid.unitLength + level.grid.origin
                         };
            Handles.DrawSolidRectangleWithOutline(verts, new Color(0, 1, 0, 0.5f), Color.black);
            Handles.Label(((rectInt.xMin + rectInt.width / 2.0f) * Vector3.right + (rectInt.yMin + rectInt.height / 2.0f) * Vector3.up) * level.grid.unitLength + level.grid.origin, "Safe Area", style);
        }
    }
}
