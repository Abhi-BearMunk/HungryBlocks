using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridManager grid;
    //public GridVisualizer gridVisualizer;
    public GameObject waveManagerObject;
    public IWaveManager waveManager;
    public List<RectInt> safeSpawnRegions;

    public bool paused;

    void Start()
    {
        waveManager = waveManagerObject.GetComponent<IWaveManager>();
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
