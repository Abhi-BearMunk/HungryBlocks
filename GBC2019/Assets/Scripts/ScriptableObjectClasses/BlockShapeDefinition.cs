using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shape Definition", menuName = "Block Shape Definition")]
public class BlockShapeDefinition : ScriptableObject
{
    public string shapeName = "Block Shape";
    public List<Vector2Int> shapeDefinition = new List<Vector2Int>();
}
