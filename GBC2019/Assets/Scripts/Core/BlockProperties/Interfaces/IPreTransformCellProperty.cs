using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPreTransformCellProperty
{
    void PreTransform(Cell cell, Vector2Int positionToCheck);
}
