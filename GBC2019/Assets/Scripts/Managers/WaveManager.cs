﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;
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
        SpawnPlayer();
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
            GameObject go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(PauseController));
            block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(Random.Range(10, grid.GetWidth() - 10), Random.Range(10, grid.GetHeight() - 10)), Block.CellType.Enemy, (Block.CellSubType)(Random.Range(1, 5)), go);
            block.gameObject.AddComponent<BlockMover>();
            //block.gameObject.AddComponent<GameObjectEntity>();
            block.GetComponent<BlockMover>().Translate(new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)));
        }
    }

    void SpawnColliding()
    {
        Block block;
        GameObject go;

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() - 60, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbMatchingSubtype>().priority = 5;
        block.GetComponent<AbsorbableByMatchingSubType>().priority = 5;
        block.GetComponent<BlockMover>().Translate(-1, 0);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(60, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbMatchingSubtype>().priority = 1;
        block.GetComponent<AbsorbableByMatchingSubType>().priority = 1;
        block.GetComponent<BlockMover>().Translate(1, 0);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() / 2, grid.GetHeight() - 40), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbMatchingSubtype>().priority = 2;
        block.GetComponent<AbsorbableByMatchingSubType>().priority = 2;
        block.GetComponent<BlockMover>().Translate(0, 1);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() / 2, 40), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbMatchingSubtype>().priority = 3;
        block.GetComponent<AbsorbableByMatchingSubType>().priority = 3;
        block.GetComponent<BlockMover>().Translate(0, -1);
    }

    void SpawnPlayer()
    {
        Block block;
        GameObject go;

        go = new GameObject("Player", 
                            typeof(PauseController),
                            typeof(KillableByNonMatchingSubType), 
                            typeof(KillNonMatchingSubType), 
                            typeof(AbsorbMatchingSubtype), 
                            typeof(BlockMover),
                            typeof(MovementControl));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() - 60, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.GetComponent<AbsorbMatchingSubtype>().priority = 100;
        block.GetComponent<BlockMover>().deltaTime = 0.1f;
    }
}
