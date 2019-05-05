using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Generates waves of NPC Blocks
/// </summary>
public class WaveManager : MonoBehaviour
{
    public GridManager grid;
    private bool spawnWave;

    public int initialNumber = 10;

    public float spawnTime = 1;
    private float spawnTimer = 0;

    private int waveCount = 0;

    public ShapeDictionary.BlockShape shape;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //spawnTimer += Time.deltaTime;
        //if(spawnWave && spawnTimer >= spawnTime)
        //{
        //    // Left
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(-10, grid.GetHeight() - 10), (Cell.CellType)(Random.Range(0, 3)));
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(-10, 10), (Cell.CellType)(Random.Range(0, 3)));
        //    // Up
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(10, grid.GetHeight() + 10), (Cell.CellType)(Random.Range(0, 3)));
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() - 10, grid.GetHeight() + 10), (Cell.CellType)(Random.Range(0, 3)));
        //    // Down
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(10, -10), (Cell.CellType)(Random.Range(0, 3)));
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() - 10, -10), (Cell.CellType)(Random.Range(0, 3)));
        //    // Right
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() + 10, grid.GetHeight() - 10), (Cell.CellType)(Random.Range(0, 3)));
        //    grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(0, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() + 10, 10), (Cell.CellType)(Random.Range(0, 3)));

        //    spawnTimer = 0;
        //}
    }

    public void InitiateWave()
    {
        spawnWave = true;
        //grid.CreateBlock(ShapeDictionary.shapeDefinitions[shape], new Vector2Int(0, 0), (Cell.CellType)(Random.Range(0, 4)));

        //grid.CreateBlock(ShapeDictionary.shapeDefinitions[shape], new Vector2Int(grid.GetWidth() / 2, grid.GetHeight() / 2), Block.CellType.Enemy, (Block.CellSubType)(Random.Range(1, 4)));

        SpawnAbunch();
        //SpawnColliding();
    }

    void SpawnWave()
    {
        waveCount++;

    }

    void SpawnAbunch()
    {
        Block block;
        for (int i = 0; i < initialNumber; i++)
        {
            GameObject go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(DestroyOnKill), typeof(WrapAround));
            block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(Random.Range(4, grid.GetWidth() - 4), Random.Range(4, grid.GetHeight() - 4)), Block.CellType.Enemy, (Block.CellSubType)(Random.Range(1, 5)), go);
            block.gameObject.AddComponent<BlockMover>();
            block.GetComponent<BlockMover>().velocity = new Vector2((float)Random.Range(-1.0f, 1.0f) / 1.0f, (float)Random.Range(-1.0f, 1.0f) / 1.0f);
        }
    }

    void SpawnColliding()
    {
        Block block;
        GameObject go;

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.H], new Vector2Int(grid.GetWidth() - 27, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().velocity = new Vector2(-1f, 0);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.H], new Vector2Int(27, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.G, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().velocity = new Vector2(1f, 0);
    }
}
