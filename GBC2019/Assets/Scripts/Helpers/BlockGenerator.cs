using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public Texture paintImage;
    public Texture eraserImage;
    [HideInInspector]
    public int width = 10;
    [HideInInspector]
    public int height = 10;
    [HideInInspector]
    public string shapeName = "";
    public BlockShapeDefinition loadBlock;

    [HideInInspector]
    public List<Vector2Int> positions = new List<Vector2Int>();


}
