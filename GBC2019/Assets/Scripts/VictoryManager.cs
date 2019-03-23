using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManager : MonoBehaviour {
    public List<BlockShapes.BlockShape> winningShapes = new List<BlockShapes.BlockShape>();
    public List<Vector2Int> winningPoints = new List<Vector2Int>();
    public GameObject winCell;

    static VictoryManager instance;

    public bool win = false;
    public GameObject winUI;

    public AudioClip victoryAudio;
    public AudioSource src;

    public static VictoryManager Instance()
    {
        return instance;
    }

    public void DestryInstance()
    {
        Destroy(instance);
        instance = null;
    }
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        GenerateWin();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Win()
    {
        win = true;
        src.clip = victoryAudio;
        src.Play();
    }

    void GenerateWin()
    {
        int startX = Random.Range(6, LevelGrid.Instance().width - 10);
        int startY = Random.Range(4, LevelGrid.Instance().height - 8);
        bool[,] shape = BlockShapes.shapeDefinitions[winningShapes[Random.Range(0, winningShapes.Count)]];
        for(int i = 0; i < shape.GetLength(0); i++)
        {
            for (int j = 0; j < shape.GetLength(1); j++)
            {
                if(shape[i, j])
                {
                    winningPoints.Add(new Vector2Int(startX + i, startY + j));
                    Instantiate(winCell, new Vector3(startX + i, startY + j, 0), Quaternion.identity);
                }
            }
        }
    }
}
