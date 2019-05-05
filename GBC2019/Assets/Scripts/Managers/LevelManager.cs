using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridManager grid;
    //public GridVisualizer gridVisualizer;
    public WaveManager waveManager;
    // Start is called before the first frame update
    void Start()
    {
        StartLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartLevel()
    {
        grid.InitializeGrid();
        //gridVisualizer.SpawnGridVisuals();
        waveManager.InitiateWave();
    }
}
