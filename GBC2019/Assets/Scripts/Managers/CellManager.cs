using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.LogWarning("Grid Manager not found! Creating a new one with default values");
            new GameObject("Grid Manager", typeof(CellManager));
        }
        return instance;
    }
}
