using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GridManager grid;
    //public GridVisualizer gridVisualizer;
    public GameObject waveManagerObject;
    public GameObject waveManagerObject2;
    public IWaveManager waveManager;
    public IWaveManager waveManager2;
    public List<RectInt> safeSpawnRegions;

    public bool paused;

    void Start()
    {
        waveManager = waveManagerObject.GetComponent<IWaveManager>();
        if(waveManagerObject2)
        {
            waveManager2 = waveManagerObject2.GetComponent<IWaveManager>();
        }
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
        if(waveManager2 != null)
        {
            waveManager2.InitiateWave();
        }
    }
}
