using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data describing absorption hierarchy
/// </summary>
public class AbsorbData : MonoBehaviour
{
    /// <summary>
    /// A type to uniquely identify this block
    /// </summary>
    public string absorbType;
    /// <summary>
    /// Only higher or equal priority can absorb
    /// </summary>    
    public uint priority;
    /// <summary>
    /// Any absorb script on this block will ignore these types
    /// </summary>
    public List<string> ignoreTypes = new List<string>();
}
