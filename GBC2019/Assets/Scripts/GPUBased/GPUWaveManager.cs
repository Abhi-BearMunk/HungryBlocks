using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUWaveManager : MonoBehaviour, IWaveManager
{
    public GridComputeOperator gridOperator;
    public GridManager grid;
    public LevelManager level;
    public PlayerMovementController playerController;
    public int seed;
    private bool spawnWave;

    public int initialNumber = 10;

    public float spawnTime = 1;
    private float spawnTimer = 0;

    private int waveCount = 0;

    public GameObject playerObject;

    public ShapeDictionary.BlockShape shape;
    int spawnPowerUp = 0;

    public bool specialShape;
    bool playerSpawned;
    int playerSubType;
    // Start is called before the first frame update
    void Start()
    {
        //specialShape = true;
        //Random.InitState(seed);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnWave && spawnTimer <= 0)
        {
            Spawn8();
            if(!playerSpawned)
            {
                SpawnPlayer();

                playerSpawned = true;
            }
            spawnPowerUp++;
            if (spawnPowerUp > 1)
            {
                spawnPowerUp = 0;
                //SpawnBoss();
                //SpawnPowerUp();
            }
            spawnTimer = spawnTime;
        }
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
        //SpawnPlayer();
        //SpawnAbunch();
        //SpawnColliding();
    }

    void SpawnWave()
    {
        waveCount++;

    }

    void SpawnBoss()
    {
        BlockProperties properties;
        //properties.moveTicks = 10;
        properties.moveTicks = 2;
        properties.velocityX = 0;
        properties.velocityY = -1;
        properties.type = Block.CellType.Enemy;
        properties.subType = (Block.CellSubType.R);
        properties.absorbPriority = 100;
        properties.absorbType = 0;
        properties.ignoreType = -1;
        properties.canAbsorb = 1;
        properties.CanBeAbsorbed = 1;
        properties.KillNonMatching = 1;
        properties.KillableByNonMatching = 1;
        properties.isGrenade = 0;
        int rand = Random.Range(0, 4);
        int subType = (playerSubType - 1) + Random.Range(1, 4);
        subType = subType % 4;
        properties.subType = (Block.CellSubType)(subType + 1);
        switch (rand)
        {
            case 0:
                gridOperator.CreateBlock(ShapeDictionary.BlockShape.SkullB3, new Vector2Int(gridOperator.width / 2, gridOperator.height + 40), properties);
                break;
            case 1:
                properties.velocityX = 1;
                properties.velocityY = 0;
                //properties.subType = (Block.CellSubType.G);
                gridOperator.CreateBlock(ShapeDictionary.BlockShape.SkullB3, new Vector2Int(-30, gridOperator.height / 2), properties);
                break;
            case 2:
                properties.velocityX = -1;
                properties.velocityY = 0;
                //properties.subType = (Block.CellSubType.B);
                gridOperator.CreateBlock(ShapeDictionary.BlockShape.SkullB3, new Vector2Int(gridOperator.width + 30, gridOperator.height / 2), properties);
                break;
            case 3:
                properties.velocityX = 0;
                properties.velocityY = 1;
                //properties.subType = (Block.CellSubType.Y);
                gridOperator.CreateBlock(ShapeDictionary.BlockShape.SkullB3, new Vector2Int(gridOperator.width / 2, -40), properties);
                break;
        }
    }

    void SpawnAbunch()
    {
        for (int i = 0; i < initialNumber; i++)
        {
            BlockProperties properties;
            properties.moveTicks = Random.Range(20, 61);
            properties.velocityX = Random.Range(0, 1.0f) > 0.5f ? -1 : 1;
            //properties.velocityX = 0;
            properties.velocityY = Random.Range(0, 1.0f) > 0.5f ? -1 : 1;
            //properties.velocityY = 0;
            properties.type = Block.CellType.Enemy;
            properties.subType = (Block.CellSubType)(Random.Range(1, 5));
            //properties.subType = Block.CellSubType.R;
            properties.absorbPriority = 0;
            properties.absorbType = 0;
            properties.ignoreType = 0;
            properties.canAbsorb = 1;
            properties.CanBeAbsorbed = 1;
            properties.KillNonMatching = 1;
            properties.KillableByNonMatching = 1;
            properties.isGrenade = 0;

            gridOperator.CreateBlock((ShapeDictionary.BlockShape)Random.Range(1,8), new Vector2Int(Random.Range(0, gridOperator.width), Random.Range(0, gridOperator.height)), properties);
        }
    }

    void SpawnColliding()
    {
        Block block;
        GameObject go;

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(AbsorbData));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() - 60, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().Translate(-1, 0);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(AbsorbData));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(60, grid.GetHeight() / 2), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().Translate(1, 0);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(AbsorbData));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() / 2, grid.GetHeight() - 40), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().Translate(0, 1);

        go = new GameObject("Block", typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbMatchingSubtype), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(AbsorbData));
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)Random.Range(1, (int)ShapeDictionary.BlockShape.Count - 1)], new Vector2Int(grid.GetWidth() / 2, 40), Block.CellType.Enemy, Block.CellSubType.R, go);
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<BlockMover>().Translate(0, -1);
    }

    List<Block.CellSubType> SubTypePermutation()
    {
        List<Block.CellSubType> subtypes = new List<Block.CellSubType>() { Block.CellSubType.R, Block.CellSubType.G, Block.CellSubType.B, Block.CellSubType.Y };
        List<Block.CellSubType> permutation = new List<Block.CellSubType>();
        Block.CellSubType temp;
        for (int i = 0; i < 4; i++)
        {
            temp = subtypes[Random.Range(0, subtypes.Count)];
            subtypes.Remove(temp);
            permutation.Add(temp);
        }
        return permutation;
    }

    void Spawn8()
    {
        ShapeDictionary.BlockShape shape;
        if(!specialShape)
        {
            shape = (ShapeDictionary.BlockShape)Random.Range(1, 8);
        }
        else
        {
            shape = ShapeDictionary.BlockShape.SkullB2;
        }


        List<Block.CellSubType> permutation1 = SubTypePermutation();
        List<Block.CellSubType> permutation2 = SubTypePermutation();

        BlockProperties properties;
        properties.moveTicks = Random.Range(80, 101);
        //properties.moveTicks = Random.Range(10, 21);
        properties.velocityX = 0;
        properties.velocityY = 0;
        properties.type = Block.CellType.Enemy;
        properties.subType = (Block.CellSubType)(Random.Range(1, 5));
        //properties.subType = Block.CellSubType.R;
        properties.absorbPriority = 0;
        properties.absorbType = 0;
        properties.ignoreType = -1;
        properties.canAbsorb = 1;
        properties.CanBeAbsorbed = 1;
        properties.KillNonMatching = 1;
        properties.KillableByNonMatching = 1;
        properties.isGrenade = 0;

        for (int i = 0; i < 4; i++)
        {
            // Y
            properties.subType = permutation1[i];
            properties.velocityX = 0;
            properties.velocityY = i % 2 == 0 ? -1 : 1;
            gridOperator.CreateBlock(shape, new Vector2Int(Random.Range(-gridOperator.width / 8, gridOperator.width / 8) + gridOperator.width / 2 + (i <= 1 ? 1 : -1) * gridOperator.width / 4, gridOperator.height / 2 + (i % 2 == 0 ? 1 : -1) * (gridOperator.height / 2 + 4)), properties);

            // X
            properties.subType = permutation2[i];
            properties.velocityX = i % 2 == 0 ? -1 : 1;
            properties.velocityY = 0;
            int id = gridOperator.CreateBlock(shape, new Vector2Int(gridOperator.width / 2 + (i % 2 == 0 ? 1 : -1) * (gridOperator.width / 2 + 5), Random.Range(-gridOperator.height / 8, gridOperator.height / 8) + gridOperator.height / 2 + (i <= 1 ? 1 : -1) * gridOperator.height / 4), properties);
            //if(playerController.playerId == 0)
            //{
            //    playerController.playerId = id;
            //}
        }
    }

    void SpawnPlayer()
    {
        BlockProperties properties;
        properties.moveTicks = 3;
        //properties.moveTicks = 1;
        properties.velocityX = 0;
        properties.velocityY = 0;
        properties.type = Block.CellType.Player;
        properties.subType = (Block.CellSubType)(Random.Range(1, 5));
        playerSubType = (int)properties.subType;
        properties.absorbPriority = 100;
        properties.absorbType = 1;
        properties.ignoreType = 2;
        properties.canAbsorb = 1;
        properties.CanBeAbsorbed = 0;
        properties.KillNonMatching = 1;
        properties.KillableByNonMatching = 1;
        properties.isGrenade = 0;

        playerController.playerId = gridOperator.CreateBlock(ShapeDictionary.BlockShape.SkullB, new Vector2Int(gridOperator.width / 2, gridOperator.height / 2), properties);
        gridOperator.player1ID = playerController.playerId;
        gridOperator.GetComponent<PlayerShootController>().playerType = (Block.CellSubType)properties.subType;
    }

    Vector2Int SafeSpot()
    {
        RectInt rect = level.safeSpawnRegions[Random.Range(0, level.safeSpawnRegions.Count)];
        return rect.position + new Vector2Int(Random.Range(0, rect.width), Random.Range(0, rect.height));
    }

    void SpawnPowerUp()
    {
        GameObject go = new GameObject("Block", typeof(Block), typeof(DestroyOnKill), typeof(PauseController), typeof(PowerUp), typeof(Sparkle));
        Block block = go.GetComponent<Block>();
        //block.gameObject.AddComponent<BlockMover>();
        //block.GetComponent<BlockMover>().deltaTime *= 2;
        if (Random.Range(0, 1.0f) >= 0.5f)
        {
            block.GetComponent<PowerUp>().weaponType = PowerUp.PowerUpType.GrenadeLauncher;
        }
        else
        {
            block.GetComponent<PowerUp>().weaponType = PowerUp.PowerUpType.Sniper;
        }

        int dir = Random.Range(1, 5);
        int shapeNo = Random.Range(0.0f, 100.0f) < 20 ? 10 : Random.Range(8, 10);
        // Left
        if (dir == 1)
        {
            //block.GetComponent<BlockMover>().Translate(1, 0);
            grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)shapeNo], SafeSpot(), Block.CellType.PowerUp, Block.CellSubType.Default, go);
        }
        // Right
        else if (dir == 2)
        {
            //block.GetComponent<BlockMover>().Translate(-1, 0);
            grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)shapeNo], SafeSpot(), Block.CellType.PowerUp, Block.CellSubType.Default, go);
            block.GetShape().Rotate(1);
            block.GetShape().Rotate(1);
        }
        // Up
        else if (dir == 3)
        {
            //block.GetComponent<BlockMover>().Translate(0, -1);
            grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)shapeNo], SafeSpot(), Block.CellType.PowerUp, Block.CellSubType.Default, go);
            block.GetShape().Rotate(1);
        }
        // Down
        if (dir == 4)
        {
            //block.GetComponent<BlockMover>().Translate(0, 1);
            grid.CreateBlock(ShapeDictionary.shapeDefinitions[(ShapeDictionary.BlockShape)shapeNo], SafeSpot(), Block.CellType.PowerUp, Block.CellSubType.Default, go);
            block.GetShape().Rotate(-1);
        }

    }
}
