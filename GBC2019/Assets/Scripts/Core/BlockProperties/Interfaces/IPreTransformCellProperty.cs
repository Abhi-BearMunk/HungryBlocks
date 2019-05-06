using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPreTransformCellProperty
{
    bool PreTransform(Cell cell, Vector2Int positionToCheck);
}
