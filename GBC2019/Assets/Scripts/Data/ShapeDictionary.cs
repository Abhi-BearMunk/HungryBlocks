using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShapeDictionary : MonoBehaviour
{

    public enum BlockShape { Dot = 0, Cross = 1, Hline = 2, Vline = 3, Box2x2 = 4, H = 5, Trident = 6, UFO = 7, Count = 8 }
    public static Dictionary<BlockShape, List<Vector2Int>> shapeDefinitions = new Dictionary<BlockShape, List<Vector2Int>>();

    private void Awake()
    {
        shapeDefinitions.Clear();
        List<Vector2Int> shapeGrid;

        // Dot
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 0));
        shapeDefinitions.Add(BlockShape.Dot, shapeGrid);

        // Cross
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(1, 0));
        shapeGrid.Add(new Vector2Int(0, 1));
        shapeGrid.Add(new Vector2Int(1, 1));
        shapeGrid.Add(new Vector2Int(2, 1));
        shapeGrid.Add(new Vector2Int(1, 2));
        shapeDefinitions.Add(BlockShape.Cross, shapeGrid);

        // Hline
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 0));
        shapeGrid.Add(new Vector2Int(1, 0));
        shapeGrid.Add(new Vector2Int(2, 0));
        shapeDefinitions.Add(BlockShape.Hline, shapeGrid);

        // Vline
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 0));
        shapeGrid.Add(new Vector2Int(0, 1));
        shapeGrid.Add(new Vector2Int(0, 2));
        shapeDefinitions.Add(BlockShape.Vline, shapeGrid);

        // Box2x2
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 0));
        shapeGrid.Add(new Vector2Int(0, 1));
        shapeGrid.Add(new Vector2Int(1, 0));
        shapeGrid.Add(new Vector2Int(1, 1));
        shapeDefinitions.Add(BlockShape.Box2x2, shapeGrid);

        // H
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 0));
        shapeGrid.Add(new Vector2Int(0, 1));
        shapeGrid.Add(new Vector2Int(0, 2));
        shapeGrid.Add(new Vector2Int(1, 1));
        shapeGrid.Add(new Vector2Int(2, 0));
        shapeGrid.Add(new Vector2Int(2, 1));
        shapeGrid.Add(new Vector2Int(2, 2));
        shapeDefinitions.Add(BlockShape.H, shapeGrid);

        // Trident
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(2, 0));
        shapeGrid.Add(new Vector2Int(2, 1));
        shapeGrid.Add(new Vector2Int(2, 2));
        shapeGrid.Add(new Vector2Int(2, 3));
        shapeGrid.Add(new Vector2Int(0, 2));
        shapeGrid.Add(new Vector2Int(1, 2));
        shapeGrid.Add(new Vector2Int(3, 2));
        shapeGrid.Add(new Vector2Int(4, 2));
        shapeGrid.Add(new Vector2Int(0, 3));
        shapeGrid.Add(new Vector2Int(4, 3));

        shapeDefinitions.Add(BlockShape.Trident, shapeGrid);

        // UFO
        shapeGrid = new List<Vector2Int>();
        shapeGrid.Add(new Vector2Int(0, 1));
        shapeGrid.Add(new Vector2Int(1, 1));
        shapeGrid.Add(new Vector2Int(2, 1));
        shapeGrid.Add(new Vector2Int(3, 1));
        shapeGrid.Add(new Vector2Int(4, 1));
        shapeGrid.Add(new Vector2Int(5, 1));
        shapeGrid.Add(new Vector2Int(6, 1));

        shapeGrid.Add(new Vector2Int(2, 0));
        shapeGrid.Add(new Vector2Int(3, 0));
        shapeGrid.Add(new Vector2Int(4, 0));

        shapeGrid.Add(new Vector2Int(2, 2));
        shapeGrid.Add(new Vector2Int(3, 2));
        shapeGrid.Add(new Vector2Int(4, 2));


        shapeDefinitions.Add(BlockShape.UFO, shapeGrid);
    }
}

