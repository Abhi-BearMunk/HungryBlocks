using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckBlockedProperty
{
    bool IsBlocked(Vector2Int position);
}
