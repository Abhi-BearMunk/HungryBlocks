using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Use this !!!
public class CellManager : Pool<Cell>
{
    private static CellManager instance;

    private void Awake()
    {
        instance = this;
    }

    // <summary>
    // Singleton Instance of CellManager
    // </summary>
    public static CellManager Instance()
    {
        if (instance == null)
        {
            Debug.LogWarning("Cell Manager not found! Creating a new one with default values");
            new GameObject("Cell Manager", typeof(CellManager));
        }
        return instance;
    }
}
