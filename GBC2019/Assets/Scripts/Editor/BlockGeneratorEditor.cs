using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockGenerator))]
public class BlockGeneratorEditor : Editor
{
    bool draw;
    Vector2 drawPos;
    bool erase;
    Vector2 erasePos;
    bool selection;
    Vector2 selectionStart;
    Vector2 selectionEnd;
    Vector2 selectionPos;
    Vector2 selectionMovePos;
    Vector2Int selectionCenter;
    Vector2Int selectionLeft;
    Vector2Int selectionRight;
    Vector2Int selectionTop;
    Vector2Int selectionBottom;
    Rect selectionRect;
    bool copyingSelection;
    bool cutSelection;
    List<Vector2Int> copyCutList = new List<Vector2Int>();
    List<Vector2Int> selected = new List<Vector2Int>();
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    LinkedList<List<Vector2Int>> undos = new LinkedList<List<Vector2Int>>();


    // Inspector gui
    bool saving = false;
    bool loading = false;

    bool shouldloadingBeShown = true;
    bool shouldSavingBeShown = true;

    bool nameEmpty = true;
    bool exists = true;

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //    BlockGenerator blockGenerator = target as BlockGenerator;

    //}


    void OnSceneGUI()
    {
        Vector3[] verts;
        BlockGenerator blockGenerator = target as BlockGenerator;

        // Draw the grid
        Handles.color = Color.cyan;
        for (int i = 0; i <= blockGenerator.height; i++)
        {
            Handles.DrawLine(i * Vector3.up, i * Vector3.up + blockGenerator.width * Vector3.right);
        }

        for (int i = 0; i <= blockGenerator.width; i++)
        {
            Handles.DrawLine(i * Vector3.right, i * Vector3.right + blockGenerator.height * Vector3.up);
        }

        // Click draw and erase
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D)
        {
            Vector2 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector2Int intPos = new Vector2Int((int)Mathf.Floor(position.x), (int)Mathf.Floor(position.y));
            CaptureCurrent(blockGenerator);
            if (!blockGenerator.positions.Contains(intPos) && InRange(intPos, blockGenerator))
            {
                blockGenerator.positions.Add(intPos);
            }
            else
            {
                blockGenerator.positions.Remove(intPos);
            }
            Event.current.Use();
        }

       
        // Selection
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        if (!selection && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftAlt)
        {
            selection = true;
            copyingSelection = false;
            cutSelection = false;
            selectionLeft = new Vector2Int(blockGenerator.width + 1, 0);
            selectionRight = new Vector2Int(-1, 0);
            selectionTop = new Vector2Int(0, -1);
            selectionBottom = new Vector2Int(0, blockGenerator.height + 1);
            selectionStart = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            selectionPos = selectionStart;
            Event.current.Use();
        }
        GUIStyle selectionTextStyle = new GUIStyle();
        selectionTextStyle.fontSize = 20;
        selectionTextStyle.alignment = TextAnchor.MiddleCenter;
        if (selection)
        {
            EditorGUI.BeginChangeCheck();
            Vector2 position = Handles.FreeMoveHandle(selectionPos, Quaternion.identity, 0.5f, Vector3.one * 1.0f, Handles.DotHandleCap);
            //Vector2 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            //Vector2 position = selectionPos;
            xMin = Mathf.Min(selectionStart.x, position.x);
            xMax = Mathf.Max(selectionStart.x, position.x);
            yMin = Mathf.Min(selectionStart.y, position.y);
            yMax = Mathf.Max(selectionStart.y, position.y);

            selectionRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);           

            // Fill Selection
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                CaptureCurrent(blockGenerator);
                selection = false;
                for (int i = (int)Mathf.Floor(selectionRect.x); i < Mathf.CeilToInt(selectionRect.x + selectionRect.width); i++)
                {
                    for (int j = (int)Mathf.Floor(selectionRect.y); j < Mathf.CeilToInt(selectionRect.y + selectionRect.height); j++)
                    {
                        Vector2Int pos = new Vector2Int(i, j);
                        if (!blockGenerator.positions.Contains(pos) && InRange(pos, blockGenerator))
                        {
                            blockGenerator.positions.Add(pos);
                        }
                    }                    
                }
                selected.Clear();
                Event.current.Use();
            }

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.LeftAlt)
            {
                selection = false;
                Event.current.Use();
            }
           
            selectionPos = position;
        }

        // Erase Selection
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.P)
        {
            CaptureCurrent(blockGenerator);
            selection = false;
            foreach (Vector2Int pos in selected)
            {
                blockGenerator.positions.Remove(pos);
            }
            selected.Clear();
            Event.current.Use();
        }

        // Draw the current positions
        if(selection)
        {
            selected.Clear();
        }
        foreach (Vector2Int position in blockGenerator.positions)
        {
            verts = new Vector3[]
                        {
                            (Vector3)(Vector2)position,
                            (Vector3)(Vector2)position + Vector3.up,
                            (Vector3)(Vector2)position + Vector3.up + Vector3.right,
                            (Vector3)(Vector2)position + Vector3.right
                         };
            if(selection && InSelectionRect(position))
            {
                selected.Add(position);
                if(position.x < selectionLeft.x)
                {
                    selectionLeft = position;
                }
                if (position.x > selectionRight.x)
                {
                    selectionRight = position;
                }
                if (position.y < selectionBottom.y)
                {
                    selectionBottom = position;
                }
                if (position.y > selectionTop.y)
                {
                    selectionTop = position;
                }
            }
            if(selected.Contains(position))
            {
                Handles.DrawSolidRectangleWithOutline(verts, Color.cyan, Color.black);
            }
            else
            {
                Handles.DrawSolidRectangleWithOutline(verts, Color.green, Color.black);
            }
        }
        
        // Copy
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
        {
            if(selected.Count > 0)
            {
                copyingSelection = true;
                cutSelection = false;
                copyCutList = new List<Vector2Int>(selected);
                //selectionMovePos = new Vector2(selectionRect.x + selectionRect.width / 2, selectionRect.y + selectionRect.height / 2);
                selectionMovePos = new Vector2((selectionLeft.x + selectionRight.x) / 2 + 1, (selectionBottom.y + selectionTop.y) / 2 + 1);
                selectionCenter = new Vector2Int((int)Mathf.Floor(selectionMovePos.x), (int)Mathf.Floor(selectionMovePos.y));
            }
            else
            {
                copyingSelection = false;
            }
            Event.current.Use();
        }

        if (copyingSelection)
        {
            EditorGUI.BeginChangeCheck();

            // Rotate
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
            {
                for(int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = RotateAboutSelectionCenter(copyCutList[i]);
                }
                Vector2Int tempLeft = selectionLeft;
                Vector2Int tempRight = selectionRight;
                Vector2Int tempBottom = selectionBottom;
                Vector2Int tempTop = selectionTop;
                selectionLeft = RotateAboutSelectionCenter(tempBottom);
                selectionRight = RotateAboutSelectionCenter(tempTop);
                selectionTop = RotateAboutSelectionCenter(tempLeft);
                selectionBottom = RotateAboutSelectionCenter(tempRight);
                Event.current.Use();
            }

            // Vertical Reflection
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.N)
            {
                for (int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = ReflectVertical(copyCutList[i]);
                }
                Vector2Int tempBottom = selectionBottom;
                Vector2Int tempTop = selectionTop;
                selectionBottom = ReflectVertical(tempTop);
                selectionTop = ReflectVertical(tempBottom);        
                Event.current.Use();
            }

            // Horizontal Reflection
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.M)
            {
                for (int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = ReflectHorizontal(copyCutList[i]);
                }
                Vector2Int tempLeft = selectionLeft;
                Vector2Int tempRight = selectionRight;
                selectionLeft = ReflectHorizontal(tempRight);
                selectionRight = ReflectHorizontal(tempLeft);
                Event.current.Use();
            }

            Vector2Int intPos = new Vector2Int((int)Mathf.Floor(selectionMovePos.x), (int)Mathf.Floor(selectionMovePos.y));
            foreach (Vector2Int pos in copyCutList)
            {
                Vector2Int position = pos + intPos - selectionCenter;
                verts = new Vector3[]
                            {
                            (Vector3)(Vector2)position,
                            (Vector3)(Vector2)position + Vector3.up,
                            (Vector3)(Vector2)position + Vector3.up + Vector3.right,
                            (Vector3)(Vector2)position + Vector3.right
                             };
                Handles.DrawSolidRectangleWithOutline(verts, Color.gray * new Color(1, 1, 1, 0.3f), Color.cyan);
            }
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(0, intPos.y, 0), new Vector2((selectionLeft + intPos - selectionCenter).x, intPos.y));
            Handles.Label(new Vector3((selectionLeft + intPos - selectionCenter).x / 2.0f, intPos.y + 1, 0), (selectionLeft + intPos - selectionCenter).x.ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(blockGenerator.width, intPos.y, 0), new Vector2((selectionRight + intPos - selectionCenter).x + 1 , intPos.y));
            Handles.Label(new Vector3((blockGenerator.width + (selectionRight + intPos - selectionCenter).x + 1) / 2.0f, intPos.y + 1, 0), (blockGenerator.width - (selectionRight + intPos - selectionCenter).x - 1).ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(intPos.x, blockGenerator.height, 0), new Vector2(intPos.x, (selectionTop + intPos - selectionCenter).y + 1));
            Handles.Label(new Vector3(intPos.x, (blockGenerator.height + (selectionTop + intPos - selectionCenter).y + 1) / 2.0f, 0), (blockGenerator.height - (selectionTop + intPos - selectionCenter).y - 1).ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(intPos.x, 0, 0), new Vector2(intPos.x, (selectionBottom + intPos - selectionCenter).y));
            Handles.Label(new Vector3(intPos.x, (selectionBottom + intPos - selectionCenter).y / 2.0f, 0), (selectionBottom + intPos - selectionCenter).y.ToString(), selectionTextStyle);

            Handles.color = Color.cyan;


            selectionMovePos = Handles.FreeMoveHandle((Vector3)selectionMovePos, Quaternion.identity, 0.5f, Vector3.one * 1.0f, Handles.DotHandleCap);
            // Paste
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V)
            {
                CaptureCurrent(blockGenerator);
                foreach (Vector2Int pos in copyCutList)
                {
                    Vector2Int position = pos + intPos - selectionCenter;
                    if (!blockGenerator.positions.Contains(position) && InRange(position, blockGenerator))
                    {
                        blockGenerator.positions.Add(position);
                    }
                }
                selected.Clear();
            }
            // Negetive Paste
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.B)
            {
                CaptureCurrent(blockGenerator);
                foreach (Vector2Int pos in copyCutList)
                {
                    Vector2Int position = pos + intPos - selectionCenter;
                    blockGenerator.positions.Remove(position);
                }
                selected.Clear();
            }
        }

        // Cut
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X)
        {
            if(selected.Count > 0)
            {
                cutSelection = true;
                copyingSelection = false;
                copyCutList = new List<Vector2Int>(selected);
                selectionMovePos = new Vector2((selectionLeft.x + selectionRight.x) / 2 + 1, (selectionBottom.y + selectionTop.y) / 2 + 1);
                selectionCenter = new Vector2Int((int)Mathf.Floor(selectionMovePos.x), (int)Mathf.Floor(selectionMovePos.y));
            }
            else
            {
                cutSelection = false;
            }
            Event.current.Use();
        }

        if (cutSelection)
        {
            EditorGUI.BeginChangeCheck();

            // Rotate
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
            {
                for (int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = RotateAboutSelectionCenter(copyCutList[i]);
                }
                Vector2Int tempLeft = selectionLeft;
                Vector2Int tempRight = selectionRight;
                Vector2Int tempBottom = selectionBottom;
                Vector2Int tempTop = selectionTop;
                selectionLeft = RotateAboutSelectionCenter(tempBottom);
                selectionRight = RotateAboutSelectionCenter(tempTop);
                selectionTop = RotateAboutSelectionCenter(tempLeft);
                selectionBottom = RotateAboutSelectionCenter(tempRight);
                Event.current.Use();
            }

            // Vertical Reflection
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.N)
            {
                for (int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = ReflectVertical(copyCutList[i]);
                }
                Vector2Int tempBottom = selectionBottom;
                Vector2Int tempTop = selectionTop;
                selectionBottom = ReflectVertical(tempTop);
                selectionTop = ReflectVertical(tempBottom);
                Event.current.Use();
            }

            // Horizontal Reflection
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.M)
            {
                for (int i = 0; i < copyCutList.Count; i++)
                {
                    copyCutList[i] = ReflectHorizontal(copyCutList[i]);
                }
                Vector2Int tempLeft = selectionLeft;
                Vector2Int tempRight = selectionRight;
                selectionLeft = ReflectHorizontal(tempRight);
                selectionRight = ReflectHorizontal(tempLeft);
                Event.current.Use();
            }

            Vector2Int intPos = new Vector2Int((int)Mathf.Floor(selectionMovePos.x), (int)Mathf.Floor(selectionMovePos.y));
            foreach (Vector2Int pos in copyCutList)
            {
                Vector2Int position = pos + intPos - selectionCenter;
                verts = new Vector3[]
                            {
                            (Vector3)(Vector2)position,
                            (Vector3)(Vector2)position + Vector3.up,
                            (Vector3)(Vector2)position + Vector3.up + Vector3.right,
                            (Vector3)(Vector2)position + Vector3.right
                             };
                Handles.DrawSolidRectangleWithOutline(verts, Color.blue * new Color(1, 1, 1, 0.3f), Color.cyan);
            }

            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(0, intPos.y, 0), new Vector2((selectionLeft + intPos - selectionCenter).x, intPos.y));
            Handles.Label(new Vector3((selectionLeft + intPos - selectionCenter).x / 2.0f, intPos.y + 1, 0), (selectionLeft + intPos - selectionCenter).x.ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(blockGenerator.width, intPos.y, 0), new Vector2((selectionRight + intPos - selectionCenter).x + 1, intPos.y));
            Handles.Label(new Vector3((blockGenerator.width + (selectionRight + intPos - selectionCenter).x + 1) / 2.0f, intPos.y + 1, 0), (blockGenerator.width - (selectionRight + intPos - selectionCenter).x - 1).ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(intPos.x, blockGenerator.height, 0), new Vector2(intPos.x, (selectionTop + intPos - selectionCenter).y + 1));
            Handles.Label(new Vector3(intPos.x, (blockGenerator.height + (selectionTop + intPos - selectionCenter).y + 1) / 2.0f, 0), (blockGenerator.height - (selectionTop + intPos - selectionCenter).y - 1).ToString(), selectionTextStyle);

            Handles.DrawLine(new Vector3(intPos.x, 0, 0), new Vector2(intPos.x, (selectionBottom + intPos - selectionCenter).y));
            Handles.Label(new Vector3(intPos.x, (selectionBottom + intPos - selectionCenter).y / 2.0f, 0), (selectionBottom + intPos - selectionCenter).y.ToString(), selectionTextStyle);

            Handles.color = Color.cyan;

            selectionMovePos = Handles.FreeMoveHandle((Vector3)selectionMovePos, Quaternion.identity, 0.5f, Vector3.one * 1.0f, Handles.DotHandleCap);
            // Paste
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.V)
            {
                CaptureCurrent(blockGenerator);
                foreach (Vector2Int pos in selected)
                {
                    blockGenerator.positions.Remove(pos);
                }
                selected.Clear();
                foreach (Vector2Int pos in copyCutList)
                {   
                    Vector2Int position = pos + intPos - selectionCenter;
                    if (!blockGenerator.positions.Contains(position) && InRange(position, blockGenerator))
                    {
                        blockGenerator.positions.Add(position);
                    }
                }
            }

            // Negetive Paste
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.B)
            {
                CaptureCurrent(blockGenerator);
                foreach (Vector2Int pos in selected)
                {
                    blockGenerator.positions.Remove(pos);
                }
                selected.Clear();
                foreach (Vector2Int pos in copyCutList)
                {
                    Vector2Int position = pos + intPos - selectionCenter;
                    blockGenerator.positions.Remove(position);
                }
            }
        }

        GUIStyle style = new GUIStyle();
        style.fixedHeight = 40;
        style.fixedWidth = 40;
        style.contentOffset = new Vector2(-20, -20);

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F)
        {
            if(!draw)
            {
                CaptureCurrent(blockGenerator);
            }
            draw = true;
            Event.current.Use();
        }

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.F)
        {
            draw = false;
            Event.current.Use();
        }

        GUIContent paintToolTip = new GUIContent("", blockGenerator.paintImage);

        if (draw)
        {
            copyingSelection = false;
            cutSelection = false;
            Vector2 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector2Int intPos = new Vector2Int((int)Mathf.Floor(position.x), (int)Mathf.Floor(position.y));
            Handles.Label((Vector3)position - Vector3.forward, paintToolTip, style);
            if (!blockGenerator.positions.Contains(intPos) && InRange(intPos, blockGenerator))
            {
                blockGenerator.positions.Add(intPos);
            }
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.G)
        {
            if(!erase)
            {
                CaptureCurrent(blockGenerator);
            }
            erase = true;
            Event.current.Use();
        }

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.G)
        {
            erase = false;
            Event.current.Use();
        }

        GUIContent eraseToolTip = new GUIContent("", blockGenerator.eraserImage);
        if (erase)
        {
            copyingSelection = false;
            cutSelection = false;
            Vector2 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector2Int intPos = new Vector2Int((int)Mathf.Floor(position.x), (int)Mathf.Floor(position.y));
            Handles.Label(position, eraseToolTip, style);
            blockGenerator.positions.Remove(intPos);
        }

        if(selection)
        {
            Color rectColor = Color.cyan;
            rectColor.a = 0.1f;
            Handles.DrawSolidRectangleWithOutline(selectionRect, rectColor, Color.black);
            Handles.Label(selectionRect.position + new Vector2(selectionRect.width / 2, selectionRect.height + 1.5f), (Mathf.CeilToInt(selectionRect.x + selectionRect.width) - Mathf.FloorToInt(selectionRect.x)).ToString(), selectionTextStyle);
            Handles.Label(selectionRect.position + new Vector2(-1.5f, selectionRect.height / 2), (Mathf.CeilToInt(selectionRect.y + selectionRect.height) - Mathf.FloorToInt(selectionRect.y)).ToString(), selectionTextStyle);
        }

        // Undo
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Z)
        {
            Undo(blockGenerator);
            Event.current.Use();
        }

        selectionTextStyle.alignment = TextAnchor.MiddleLeft;
        // Controls Description
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(20, 90, 350, 500));

        var rect = EditorGUILayout.BeginVertical();
        GUI.color = Color.yellow;
        GUI.Box(rect, GUIContent.none);

        GUI.color = Color.red;

        selectionTextStyle.fontStyle = FontStyle.Bold;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Controls", selectionTextStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("D (TAP) - toggle");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("F (HOLD) - Paint");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("G (HOLD) - Erase");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Left Alt (HOLD) - Click Drag handle for rectangular selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Left Alt (HOLD) + Space (TAP) - Fill Selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("P (TAP) - Delete Selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("C (TAP) - Toggle Copy");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("X (TAP) - Toggle Cut");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("V (TAP) - Paste");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("B (TAP) - Paste Negetive");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("R (TAP) - Rotate Cut / Copy selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("M (TAP) - Horizontal Flip Cut / Copy selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("N (TAP) - Vertical Flip Cut / Copy selection");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Z (TAP) - Undo");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();


        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(20, 20, 350, 500));

        GUIStyle boxStyle = new GUIStyle();
        //boxStyle.margin = new RectOffset(10, 10, 10, 10);
        var rect2 = EditorGUILayout.BeginVertical();
        GUI.color = Color.yellow;
        GUI.Box(rect2, GUIContent.none);

        GUI.color = Color.white;

        selectionTextStyle.fontStyle = FontStyle.Bold;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Properties", selectionTextStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Width ");
        blockGenerator.width = EditorGUILayout.IntField(blockGenerator.width);
        GUILayout.Label("Height ");
        blockGenerator.height = EditorGUILayout.IntField(blockGenerator.height);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh"))
        {
            Refresh(blockGenerator);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        // Saving
        GUI.backgroundColor = Color.yellow;
        GUILayout.BeginArea(new Rect(20, 359, 350, 500));
        //boxStyle.margin = new RectOffset(10, 10, 10, 10);
        var rect3 = EditorGUILayout.BeginVertical();
        GUI.color = Color.yellow;
        GUI.Box(rect3, GUIContent.none);

        GUI.color = Color.white;

        selectionTextStyle.fontStyle = FontStyle.Bold;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Load / Save", selectionTextStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();



        if (blockGenerator.loadBlock != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.TextArea(blockGenerator.loadBlock.shapeName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (shouldloadingBeShown)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.green;
                if (GUILayout.Button("Load"))
                {
                    loading = true;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.magenta;
                GUILayout.TextArea("Loading Will Override current shape! Continue?");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Yes !!!"))
                {
                    CaptureCurrent(blockGenerator);
                    blockGenerator.positions = blockGenerator.loadBlock.shapeDefinition;
                    loading = false;
                }

                GUI.color = Color.green;
                if (GUILayout.Button("No"))
                {
                    loading = false;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal();
        
        GUI.color = Color.white;
        blockGenerator.shapeName = GUILayout.TextField(blockGenerator.shapeName);
        
        GUILayout.EndHorizontal();

        if (shouldSavingBeShown)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.color = Color.green;
            if (GUILayout.Button("Save"))
            {
                saving = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            if (nameEmpty)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.color = Color.red;
                GUILayout.TextArea("Please enter shape name !!!");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (exists)
            {
                GUI.color = Color.red;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OverWrite !!!"))
                {
                    BlockShapeDefinition asset = (BlockShapeDefinition)AssetDatabase.LoadAssetAtPath("Assets/Data/" + blockGenerator.shapeName + ".asset", typeof(BlockShapeDefinition));
                    asset.shapeDefinition = blockGenerator.positions;
                    AssetDatabase.SaveAssets();

                    //EditorUtility.FocusProjectWindow();

                    Selection.activeObject = asset;
                    saving = false;
                }

                GUI.color = Color.green;
                if (GUILayout.Button("Cancel"))
                {
                    saving = false;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                BlockShapeDefinition asset = CreateInstance<BlockShapeDefinition>();
                asset.shapeName = blockGenerator.shapeName;
                asset.shapeDefinition = blockGenerator.positions;
                AssetDatabase.CreateAsset(asset, "Assets/Data/" + blockGenerator.shapeName + ".asset");
                AssetDatabase.SaveAssets();

                //EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
                saving = false;
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        Handles.EndGUI();

        if (Event.current.type == EventType.Repaint)
        {
            shouldloadingBeShown = !loading;
            shouldSavingBeShown = !saving;
        }

        if (Event.current.type == EventType.Repaint)
        {
            nameEmpty = blockGenerator.shapeName.Length == 0;
            exists = (BlockShapeDefinition)AssetDatabase.LoadAssetAtPath("Assets/Data/" + blockGenerator.shapeName + ".asset", typeof(BlockShapeDefinition)) != null;
        }
    }

    bool InRange(Vector2Int intPos, BlockGenerator generator)
    {
        return intPos.x >= 0 && intPos.y >= 0 && intPos.x < generator.width && intPos.y < generator.height;
    }

    bool InSelectionRect(Vector2Int intPos)
    {
        return intPos.x >= Mathf.Floor(xMin) && intPos.y >= Mathf.Floor(yMin) && intPos.x <= Mathf.Floor(xMax) && intPos.y <= Mathf.Floor(yMax);
    }

    Vector2Int RotateAboutSelectionCenter(Vector2Int pos)
    {
        Vector2Int current = pos - selectionCenter;
        Vector2Int rotated = new Vector2Int(current.y, -current.x);
        return selectionCenter + rotated;
    }

    Vector2Int ReflectVertical(Vector2Int pos)
    {
        Vector2Int current = pos - selectionCenter;
        current.y = -current.y;
        return selectionCenter + current;
    }

    Vector2Int ReflectHorizontal(Vector2Int pos)
    {
        Vector2Int current = pos - selectionCenter;
        current.x = -current.x;
        return selectionCenter + current;
    }

    void CaptureCurrent(BlockGenerator blockGenerator)
    {
        undos.AddLast(new List<Vector2Int>(blockGenerator.positions));
        if(undos.Count > 10000)
        {
            undos.RemoveFirst();
        }
    }

    void Undo(BlockGenerator blockGenerator)
    {
        if(undos.Count > 0)
        {
            blockGenerator.positions = new List<Vector2Int>(undos.Last.Value);
            undos.RemoveLast();
            selected.Clear();
            selection = false;
        }
    }

    void Refresh(BlockGenerator blockGenerator)
    {
        selected.Clear();
        copyCutList.Clear();
        selection = false;
        copyingSelection = false;
        cutSelection = false;
        draw = false;
        erase = false;
        CaptureCurrent(blockGenerator);
        blockGenerator.positions.Clear();
    }
}
