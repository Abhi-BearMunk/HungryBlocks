using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridManager grid;
    //public GridVisualizer gridVisualizer;
    public WaveManager waveManager;
    // Start is called before the first frame update

    public bool paused;

    void Start()
    {
        StartLevel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }
    }

    void StartLevel()
    {
        grid.InitializeGrid();
        //gridVisualizer.SpawnGridVisuals();
        waveManager.InitiateWave();
    }
}
