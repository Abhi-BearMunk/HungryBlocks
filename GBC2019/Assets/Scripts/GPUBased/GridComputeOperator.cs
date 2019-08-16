using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockProperties
{
    public Block.CellType type;
    public Block.CellSubType subType;
    public int velocityX;
    public int velocityY;
    public int moveTicks;
    public int absorbPriority;
    public int absorbType;// 0- Enemy, 1-Player1, 2 - Player1Bullets
    public int ignoreType;
    public int canAbsorb;
    public int CanBeAbsorbed;
    public int KillNonMatching;
    public int KillableByNonMatching;
    public int isGrenade;
}

public struct BlockStruct
{
    public int ID; //0
    public int creationID;//1
    public int type;//2
    public int subType;//3
    public int dead;//4
    public int left;//5
    public int right;//6
    public int up;//7
    public int down;//8
    public int centerX;//9
    public int centerY;//10
    public int velocityX;//11
    public int velocityY;//12
    public int moveTicks;//13
    public int currentTick;//14
    public int canMove;//15
    public int attachToBlockId;//16
    public int attatched;//17
    public int absorbPriority;//18
    public int absorbType;//19
    public int ignoreType;//20
    public int canAbsorb;//21
    public int CanBeAbsorbed;//22
    public int KillNonMatching;//23
    public int KillableByNonMatching;//24
    public int centerCellSquareDistance;//25
    public int centerGridID;//26
    public int centerCellID;//27
    public int rotateDirection;//28
    public int aboutToDie;//29
    public int isGrenade;//30
    public int cellCount;//31

    public static int GetLength()
    {
        return sizeof(int) * 32;
    }
}

public struct CellStruct
{
    public int ID;//0
    public int gridPosX;//1
    public int gridPosY;//2
    public int parentBlockID;//3
    public int dying;//4
    public int dead;//5
    public int left;//6
    public int right;//7
    public int up;//8
    public int down;//9
    public int rotTicks;//10
    public float lastX;//0
    public float lastY;//1
    public int connectedToCenter;//11
    public int isImpactDeath;//12
    float impactX;//2
    float impactY;//3
    float timeSinceImpact;//4

    public static int GetLength()
    {
        return sizeof(int) * 13 + sizeof(float) * 5;
    }
}

public struct GridCell
{
    public int cells0;
    public int cells1;
    public int cells2;
    public int cells3;
    public int cells4;
    public int cells5;
    public int cells6;
    public int cells7;
    public int cells8;
    public int cells9;

    public static int GetLength()
    {
        return sizeof(int) * 10;
    }
}

[System.Serializable]
public struct int2
{
    public int x;
    public int y;

    public static int GetLength()
    {
        return sizeof(int) * 2;
    }
}

struct TransformData
{
    public int blockId;
    public int velocityX;
    public int velocityY;
    public int rotation;

    public static int GetLength()
    {
        return sizeof(int) * 4;
    }
};

struct PlayerAimData
{
    int direction0;
    int direction1;
    int direction2;
    int direction3;
    int direction4;
    int direction5;
    int direction6;
    int direction7;
    int direction8;

    public static int GetLength()
    {
        return sizeof(int) * 9;
    }
};

public class GridComputeOperator : MonoBehaviour
{
    public int numberOfIterationsPerFixedUpdate = 1;
    public int width = 128;
    public int height = 72;
    public int scalingFactor = 20;
    public int grenadeDetonationRadius = 8;
    public Texture2D cellSprite;
    public Texture2D debugSprite;
    public Texture2D arrowSprite;
    public Texture2D reticleSprite;
    public Renderer rend;
    public Renderer debugRend;
    public RenderTexture displayTexture;
    public RenderTexture debugTexture;
    public ComputeShader gridCruncher;

    public int player1ID = -1;
    public int2 player1AimDirection;
    List<TransformData> movementQueue = new List<TransformData>();
    List<TransformData> rotationQueue = new List<TransformData>();
    public List<ShapeDictionary.BlockShape> shapesToCheck = new List<ShapeDictionary.BlockShape>();

    public bool doAttatchment = true;
    public bool destroyDiscoonected = true;
    public bool hardCodeDim;
    public int hardCodedDimValue = 50;
    public int currentCellDisconnect;

    BlockStruct[] blockArray;
    CellStruct[] cellArray;
    GridCell[] grid;
    int2[] maxBound;
    int[] newBlockIdArray;
    TransformData[] setVelocityArray;
    PlayerAimData[] playerAimDataArray;

    ComputeBuffer blockBuffer;
    ComputeBuffer cellBuffer;
    ComputeBuffer gridCellBuffer;

    ComputeBuffer deadBlocksBuffer;
    ComputeBuffer deadCellsBuffer;
    ComputeBuffer newCellsBuffer;
    ComputeBuffer newBlockId;

    ComputeBuffer attatchBlocksBuffer;
    ComputeBuffer attatchBlocksRetrieveCountBuffer;

    ComputeBuffer maxBlockBounds;

    ComputeBuffer setVelocityBuffer;

    ComputeBuffer playerAimDataBuffer;

    int creationId = 0;

    Dictionary<ShapeDictionary.BlockShape, ComputeBuffer> shapeDefinitionBuffers = new Dictionary<ShapeDictionary.BlockShape, ComputeBuffer>();
    Dictionary<ShapeDictionary.BlockShape, Vector2Int> shapeDefinitionCenters = new Dictionary<ShapeDictionary.BlockShape, Vector2Int>();
    Dictionary<ShapeDictionary.BlockShape, int> shapeDefinitionCount = new Dictionary<ShapeDictionary.BlockShape, int>();

    // Start is called before the first frame update
    void Awake()
    {
        // Initialize arrays
        blockArray = new BlockStruct[width * height];
        cellArray = new CellStruct[width * height];
        grid = new GridCell[width * height];
        maxBound = new int2[1];
        newBlockIdArray = new int[1];
        setVelocityArray = new TransformData[1];
        playerAimDataArray = new PlayerAimData[1];

        // Initialize buffers
        blockBuffer = new ComputeBuffer(width * height, BlockStruct.GetLength(), ComputeBufferType.Default);
        cellBuffer = new ComputeBuffer(width * height, CellStruct.GetLength(), ComputeBufferType.Default);
        gridCellBuffer = new ComputeBuffer(width * height, GridCell.GetLength(), ComputeBufferType.Default);
        newCellsBuffer = new ComputeBuffer(width * height, sizeof(int) * 2, ComputeBufferType.Default);
        deadCellsBuffer = new ComputeBuffer(width * height, sizeof(int), ComputeBufferType.Append);
        deadBlocksBuffer = new ComputeBuffer(width * height, sizeof(int), ComputeBufferType.Append);
        attatchBlocksBuffer = new ComputeBuffer(width * height, sizeof(int) * 2, ComputeBufferType.Append);
        attatchBlocksRetrieveCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        attatchBlocksBuffer.SetCounterValue(0);
        maxBlockBounds = new ComputeBuffer(1, sizeof(int) * 2, ComputeBufferType.Default);
        newBlockId = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Default);
        setVelocityBuffer = new ComputeBuffer(1, TransformData.GetLength(), ComputeBufferType.Default);
        playerAimDataBuffer = new ComputeBuffer(1, PlayerAimData.GetLength(), ComputeBufferType.Default);
        // Set Buffers
        blockBuffer.SetData(blockArray);
        cellBuffer.SetData(cellArray);
        gridCellBuffer.SetData(grid);
        maxBlockBounds.SetData(maxBound);
        newBlockId.SetData(newBlockIdArray);
        playerAimDataBuffer.SetData(playerAimDataArray);

        // Create and set render
        displayTexture = new RenderTexture(width * scalingFactor, height * scalingFactor, 24);
        displayTexture.enableRandomWrite = true;
        displayTexture.format = RenderTextureFormat.ARGB64;
        displayTexture.antiAliasing = 8;
        displayTexture.filterMode = FilterMode.Trilinear;
        displayTexture.wrapMode = TextureWrapMode.Repeat;
        displayTexture.Create();
        rend.material.mainTexture = displayTexture;

        debugTexture = new RenderTexture(width, width, 24);
        debugTexture.enableRandomWrite = true;
        debugTexture.format = RenderTextureFormat.ARGB64;
        debugTexture.Create();
        debugRend.material.mainTexture = debugTexture;

        // Set Compute shader uniforms
        gridCruncher.SetInt("width", width);
        gridCruncher.SetInt("height", height);
        gridCruncher.SetInt("scalingFactor", scalingFactor);
        gridCruncher.SetInt("detonationRadius", grenadeDetonationRadius);

        Debug.Log("Fixed Delta Time = " + Time.fixedDeltaTime);
        //Time.fixedDeltaTime /= 2; 
    }

    private void Start()
    {
        GenerateShapeBuffers();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        for(int i = 0; i < numberOfIterationsPerFixedUpdate; i++)
        {
            ComputeLoop();
        }
    }
    void ComputeLoop()
    {
        gridCruncher.SetFloat("deltaTime", Time.deltaTime);
        //gridCruncher.SetFloat("random1", Random.Range(0.0f, 1.0f));
        //gridCruncher.SetFloat("random100", Random.Range(0.0f, 100.0f));
        //gridCruncher.SetFloat("randomMinusOneToOne", Random.Range(-1.0f, 1.0f));

        foreach (TransformData data in movementQueue)
        {
            setVelocityArray[0].blockId = data.blockId;
            setVelocityArray[0].velocityX = data.velocityX;
            setVelocityArray[0].velocityY = data.velocityY;
            setVelocityArray[0].rotation = data.rotation;
            setVelocityBuffer.SetData(setVelocityArray);

            int velocityKernel = gridCruncher.FindKernel("SetVelocityById");
            gridCruncher.SetBuffer(velocityKernel, "setVelocityBuffer", setVelocityBuffer);
            gridCruncher.SetBuffer(velocityKernel, "blockBuffer", blockBuffer);
            gridCruncher.Dispatch(velocityKernel, 1, 1, 1);
        }
        movementQueue.Clear();

        foreach (TransformData data in rotationQueue)
        {
            setVelocityArray[0].blockId = data.blockId;
            setVelocityArray[0].velocityX = data.velocityX;
            setVelocityArray[0].velocityY = data.velocityY;
            setVelocityArray[0].rotation = data.rotation;
            setVelocityBuffer.SetData(setVelocityArray);

            int velocityKernel = gridCruncher.FindKernel("SetRotationById");
            gridCruncher.SetBuffer(velocityKernel, "setVelocityBuffer", setVelocityBuffer);
            gridCruncher.SetBuffer(velocityKernel, "blockBuffer", blockBuffer);
            gridCruncher.Dispatch(velocityKernel, 1, 1, 1);
        }
        rotationQueue.Clear();

        int kernel = gridCruncher.FindKernel("TickBlock");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        if (doAttatchment)
        {
            attatchBlocksBuffer.SetCounterValue(0);

            kernel = gridCruncher.FindKernel("CheckBlockCollisions");
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
            gridCruncher.SetBuffer(kernel, "attatchBlocksBuffer", attatchBlocksBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);


            ComputeBuffer.CopyCount(attatchBlocksBuffer, attatchBlocksRetrieveCountBuffer, 0);
            int[] counter = new int[1] { 0 };
            attatchBlocksRetrieveCountBuffer.GetData(counter);

            int2[] attatchBlocks = new int2[counter[0]];
            attatchBlocksBuffer.GetData(attatchBlocks);

            if (counter[0] > 0)
            {
                Dictionary<int, int> attatchBlocksDictionary = new Dictionary<int, int>();
                foreach (int2 blockPair in attatchBlocks)
                {
                    attatchBlocksDictionary[blockPair.x] = blockPair.y;
                }
                foreach (int2 blockPair in attatchBlocks)
                {
                    int value = attatchBlocksDictionary[blockPair.x];
                    while (attatchBlocksDictionary.ContainsKey(value))
                    {
                        attatchBlocksDictionary[blockPair.x] = attatchBlocksDictionary[value];
                        value = attatchBlocksDictionary[blockPair.x];
                    }
                }
                int multipleOf64Offset = 64 - (attatchBlocks.Length % 64);
                multipleOf64Offset = multipleOf64Offset == 64 ? 0 : multipleOf64Offset;

                int2[] attatchBlocksCurated = new int2[attatchBlocks.Length + multipleOf64Offset];
                for (int i = 0; i < attatchBlocks.Length; i++)
                {
                    attatchBlocksCurated[i].x = attatchBlocks[i].x;
                    attatchBlocksCurated[i].y = attatchBlocksDictionary[attatchBlocks[i].x];
                }

                for (int i = 0; i < multipleOf64Offset; i++)
                {
                    attatchBlocksCurated[attatchBlocksCurated.Length - i - 1].x = -1;
                    attatchBlocksCurated[attatchBlocksCurated.Length - i - 1].y = -1;
                }
                attatchBlocksBuffer.SetData(attatchBlocksCurated);

                kernel = gridCruncher.FindKernel("UpdateAttatchBlockID");
                gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
                gridCruncher.SetBuffer(kernel, "attatchBlocksBufferCurated", attatchBlocksBuffer);
                gridCruncher.Dispatch(kernel, attatchBlocksCurated.Length / 64, 1, 1);

                kernel = gridCruncher.FindKernel("AttatchBlockCells");
                gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
                gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
                gridCruncher.Dispatch(kernel, cellArray.Length / 8, 1, 1);
            }
        }

        kernel = gridCruncher.FindKernel("MoveCells");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        RefreshGrid();

        kernel = gridCruncher.FindKernel("PostMoveKillNonMatchingCells");
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("PostMoveKillOutgoingCells");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        RefreshGrid();
        RefreshBlocks();

        if (destroyDiscoonected)
        {
            kernel = gridCruncher.FindKernel("ResetCellConnctionsToCenter");
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

            int maxDim = 0;
            if (!hardCodeDim)
            {
                maxBlockBounds.GetData(maxBound);
                maxDim = maxBound[0].x + maxBound[0].y;
                Debug.Log("MAX DIMENSIONS : X = " + maxBound[0].x + ", Y = " + maxBound[0].y);
            }
            else
            {
                maxDim = hardCodedDimValue;
            }

            for (int i = 0; i < maxDim; i++)
            {
                kernel = gridCruncher.FindKernel("ConnectCellToCenter");
                gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
                gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
                gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);
            }

            kernel = gridCruncher.FindKernel("DestroyDisconnectedCells");
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

            RefreshBlocks();
            RefreshGrid();
        }

        kernel = gridCruncher.FindKernel("ResetBlockDeathAndMove");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("RefreshBlockDeath");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        // Grenade
        kernel = gridCruncher.FindKernel("Detonate");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, grenadeDetonationRadius * 2 + 1, grenadeDetonationRadius * 2 + 1);

        RefreshBlocks();
        RefreshGrid();

        kernel = gridCruncher.FindKernel("ResetBlockDeathAndMove");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("RefreshBlockDeath");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        // DRAW
        kernel = gridCruncher.FindKernel("ClearDisplay");
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.Dispatch(kernel, (width * scalingFactor) / 16, (height * scalingFactor) / 9, 1);

        kernel = gridCruncher.FindKernel("DisplayCells");
        gridCruncher.SetInt("player1ID", player1ID);
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.SetTexture(kernel, "cellSprite", cellSprite);
        gridCruncher.SetTexture(kernel, "debugSprite", debugSprite);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, (width * scalingFactor) / 16, (height * scalingFactor) / 9, 1);

        kernel = gridCruncher.FindKernel("DisplayDyingCells");
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.SetTexture(kernel, "cellSprite", cellSprite);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, (scalingFactor) / 2, (scalingFactor) / 2, cellArray.Length / 32);

        kernel = gridCruncher.FindKernel("LerpCells");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        if(player1AimDirection.x != 0 || player1AimDirection.y != 0 && player1ID > -1)
        {
            kernel = gridCruncher.FindKernel("DisplayAimReticle");
            gridCruncher.SetInts("aimDirection", new int[2] { player1AimDirection.x, player1AimDirection.y });
            gridCruncher.SetFloat("aimArrowNormalizedSize", 1);
            gridCruncher.SetInt("player1ID", player1ID);
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer); 
            gridCruncher.SetBuffer(kernel, "playerAimData", playerAimDataBuffer);
            gridCruncher.SetTexture(kernel, "arrowSprite", arrowSprite);
            gridCruncher.SetTexture(kernel, "reticleSprite", reticleSprite);
            gridCruncher.SetTexture(kernel, "Result", displayTexture);
            gridCruncher.Dispatch(kernel, scalingFactor / 2, scalingFactor / 2, Mathf.Max(width, height) / 32);
        }

        //kernel = gridCruncher.FindKernel("DisplayCenterPins");
        //gridCruncher.SetTexture(kernel, "Result", displayTexture);
        ////gridCruncher.SetTexture(kernel, "cellSprite", cellSprite);
        //gridCruncher.SetTexture(kernel, "debugSprite", debugSprite);
        //gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        //gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        //gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        //gridCruncher.Dispatch(kernel, (width * scalingFactor) / 16, (height * scalingFactor) / 9, 1);
    }

    private void AddShapeBuffer(ShapeDictionary.BlockShape shapeName, List<Vector2Int> shapeDefinition)
    {
        // Copy shape onto a int2 array
        int paddingToMakeItMultipleOf64 = 64 - (shapeDefinition.Count % 64);
        int2[] shape = new int2[shapeDefinition.Count + paddingToMakeItMultipleOf64];
        int index = 0;
        int centerX = 0;
        int centerY = 0;
        foreach (Vector2Int v in shapeDefinition)
        {
            shape[index].x = v.x;
            shape[index].y = v.y;
            centerX += v.x;
            centerY += v.y;
            index++;
        }

        centerX = Mathf.FloorToInt(centerX / shapeDefinition.Count);
        centerY = Mathf.FloorToInt(centerY / shapeDefinition.Count);


        for (int i = index; i < shape.Length; i++)
        {
            shape[i].x = -10000;
            shape[i].y = -10000;
        }

        ComputeBuffer shapeBuffer = new ComputeBuffer(shape.Length, int2.GetLength(), ComputeBufferType.Default);
        shapeBuffer.SetData(shape);

        shapeDefinitionBuffers.Add(shapeName, shapeBuffer);
        shapeDefinitionCenters.Add(shapeName, new Vector2Int(centerX, centerY));
        shapeDefinitionCount.Add(shapeName, shape.Length);
    }

    private void GenerateShapeBuffers()
    {
        foreach(ShapeDictionary.BlockShape shapeName in ShapeDictionary.shapeDefinitions.Keys)
        {
            AddShapeBuffer(shapeName, ShapeDictionary.shapeDefinitions[shapeName]);
        }
    }

    public int CreateBlock(ShapeDictionary.BlockShape shapeName, Vector2Int position, BlockProperties properties, int relativeBlockID = -1)
    {
        creationId++;

        Vector2Int center = shapeDefinitionCenters[shapeName];

        // Set New Parameters
        gridCruncher.SetInt("newCreationID", creationId);
        gridCruncher.SetInt("newType", (int)properties.type);
        gridCruncher.SetInt("newSubType", (int)properties.subType);
        gridCruncher.SetInt("newBlockOffsetX", center.x - position.x);
        gridCruncher.SetInt("newBlockOffsetY", center.y - position.y);
        gridCruncher.SetInt("newVelocityX", properties.velocityX);
        gridCruncher.SetInt("newVelocityY", properties.velocityY);
        gridCruncher.SetInt("newMoveTicks", properties.moveTicks);
        gridCruncher.SetInt("newAbsorbPriority", properties.absorbPriority);
        gridCruncher.SetInt("newAbsorbType", properties.absorbType);
        gridCruncher.SetInt("newIgnoreType", properties.ignoreType);
        gridCruncher.SetInt("newCanAbsorb", properties.canAbsorb);
        gridCruncher.SetInt("newCanBeAbsorbed", properties.CanBeAbsorbed);
        gridCruncher.SetInt("newKillNonMatching", properties.KillNonMatching);
        gridCruncher.SetInt("newKillableByNonMatching", properties.KillableByNonMatching);
        gridCruncher.SetInt("newIsGrenade", properties.isGrenade);
        gridCruncher.SetInt("spawnRelativeBlockID", relativeBlockID);

        // Reset the new cell buffer
        //newCellsBuffer.SetData(shape);

        // Flush deadCells and deadBlocks Buffer
        deadBlocksBuffer.SetCounterValue(0);
        deadCellsBuffer.SetCounterValue(0);

        // Find dead blocks
        int kernel = gridCruncher.FindKernel("GetDeadBlocks");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksBuffer", deadBlocksBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        // Assign properties of the new block
        kernel = gridCruncher.FindKernel("AssignBlockProperties");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.SetBuffer(kernel, "newBlockId", newBlockId);
        gridCruncher.Dispatch(kernel, 1, 1, 1);
        newBlockId.GetData(newBlockIdArray);

        // Get all the dead cells
        kernel = gridCruncher.FindKernel("GetDeadCells");
        gridCruncher.SetBuffer(kernel, "deadCellsBuffer", deadCellsBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        // Add the new cells and set their properties
        kernel = gridCruncher.FindKernel("AddCells");
        gridCruncher.SetBuffer(kernel, "newCells", shapeDefinitionBuffers[shapeName]);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        //deadCellsBuffer.SetCounterValue((uint)shape.Length);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.SetBuffer(kernel, "deadCellsCurated", deadCellsBuffer);
        gridCruncher.Dispatch(kernel, shapeDefinitionCount[shapeName] / 64, 1, 1);

        RefreshGrid();

        return newBlockIdArray[0];
    }

    public int CreateBlock(List<Vector2Int> shapeDefinition, Vector2Int position, BlockProperties properties, int relativeBlockID = -1)
    {
        creationId++;
        // Copy shape onto a int2 array
        int paddingToMakeItMultipleOf64 = 64 - (shapeDefinition.Count % 64);
        int2[] shape = new int2[shapeDefinition.Count + paddingToMakeItMultipleOf64];
        int index = 0;
        int centerX = 0;
        int centerY = 0;
        foreach(Vector2Int v in shapeDefinition)
        {
            shape[index].x = v.x + position.x;
            shape[index].y = v.y + position.y;
            centerX += v.x;
            centerY += v.y;
            index++;
        }

        centerX = Mathf.FloorToInt(centerX / shapeDefinition.Count);
        centerY = Mathf.FloorToInt(centerY / shapeDefinition.Count);


        for (int i = index; i < shape.Length; i++)
        {
            shape[i].x = -10000;
            shape[i].y = -10000;
        }

        // Set New Parameters

        gridCruncher.SetInt("newCreationID", creationId);
        gridCruncher.SetInt("newType", (int)properties.type);
        gridCruncher.SetInt("newSubType", (int)properties.subType);
        gridCruncher.SetInt("newBlockOffsetX", centerX);
        gridCruncher.SetInt("newBlockOffsetY", centerY);
        gridCruncher.SetInt("newVelocityX", properties.velocityX);
        gridCruncher.SetInt("newVelocityY", properties.velocityY);
        gridCruncher.SetInt("newMoveTicks", properties.moveTicks);
        gridCruncher.SetInt("newAbsorbPriority", properties.absorbPriority);
        gridCruncher.SetInt("newAbsorbType", properties.absorbType);
        gridCruncher.SetInt("newIgnoreType", properties.ignoreType);
        gridCruncher.SetInt("newCanAbsorb", properties.canAbsorb);
        gridCruncher.SetInt("newCanBeAbsorbed", properties.CanBeAbsorbed);
        gridCruncher.SetInt("newKillNonMatching", properties.KillNonMatching);
        gridCruncher.SetInt("newKillableByNonMatching", properties.KillableByNonMatching);
        gridCruncher.SetInt("newIsGrenade", properties.isGrenade);
        gridCruncher.SetInt("spawnRelativeBlockID", relativeBlockID);

        // Reset the new cell buffer
        newCellsBuffer.SetData(shape);

        // Flush deadCells and deadBlocks Buffer
        deadBlocksBuffer.SetCounterValue(0);
        deadCellsBuffer.SetCounterValue(0);

        // Find dead blocks
        int kernel = gridCruncher.FindKernel("GetDeadBlocks");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksBuffer", deadBlocksBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        // Assign properties of the new block
        kernel = gridCruncher.FindKernel("AssignBlockProperties");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.SetBuffer(kernel, "newBlockId", newBlockId);
        gridCruncher.Dispatch(kernel, 1, 1, 1);
        newBlockId.GetData(newBlockIdArray);

        // Get all the dead cells
        kernel = gridCruncher.FindKernel("GetDeadCells");
        gridCruncher.SetBuffer(kernel, "deadCellsBuffer", deadCellsBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        // Add the new cells and set their properties
        kernel = gridCruncher.FindKernel("AddCells");
        gridCruncher.SetBuffer(kernel, "newCells", newCellsBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        //deadCellsBuffer.SetCounterValue((uint)shape.Length);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.SetBuffer(kernel, "deadCellsCurated", deadCellsBuffer);
        gridCruncher.Dispatch(kernel, shape.Length / 64, 1, 1);

        RefreshGrid();

        return newBlockIdArray[0];
    }

    void RefreshGrid()
    {
        int kernel = gridCruncher.FindKernel("FlushGridData");
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, width / 64, height / 9, 1);

        for(int i = 0; i < 10; i++)
        {
            gridCruncher.SetInt("gridCellIndex", i);
            kernel = gridCruncher.FindKernel("UpdateGridDataAtIndex");
            gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);
        }
    }

    void RefreshBlocks()
    {
        int kernel = gridCruncher.FindKernel("FlushBlockBounds");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "maxBlockBounds", maxBlockBounds);
        gridCruncher.SetBuffer(kernel, "playerAimData", playerAimDataBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("UpdateBlockBoundsAndOtherEndPhaseData");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        //gridCruncher.SetBuffer(kernel, "playerAimData", playerAimDataBuffer);
        gridCruncher.SetInt("player1ID", player1ID);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);
        //playerAimDataBuffer.GetData(playerAimDataArray);

        kernel = gridCruncher.FindKernel("UpdateBlockCentersAndMaxBlockBound");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "maxBlockBounds", maxBlockBounds);
        gridCruncher.Dispatch(kernel, blockArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("UpdatePlayerAimReticles");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "playerAimData", playerAimDataBuffer);
        gridCruncher.SetInt("player1ID", player1ID);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("GetBlockCenterCellDistance");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("GetBlockCenterGridID");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);

        kernel = gridCruncher.FindKernel("GetBlockCenterCell");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 64, 1, 1);
    }

    public void SetVelocity(int blockID, int velocityX, int velocityY)
    {
        TransformData data;

        data.blockId = blockID;
        data.velocityX = velocityX;
        data.velocityY = velocityY;
        data.rotation = 0;

        movementQueue.Add(data);

        //FixedUpdate();

        //setVelocityBuffer.SetData(setVelocityArray);

        //int kernel = gridCruncher.FindKernel("SetVelocityAndRotationById");
        //gridCruncher.SetBuffer(kernel, "setVelocityBuffer", setVelocityBuffer);
        //gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        //gridCruncher.Dispatch(kernel, 1, 1, 1);
    }

    public void SetRotation(int blockID, int rotation)
    {
        TransformData data;

        data.blockId = blockID;
        data.velocityX = 0;
        data.velocityY = 0;
        data.rotation = rotation;

        rotationQueue.Add(data);
    }


    void OnDestroy()
    {
        blockBuffer.Release();
        cellBuffer.Release();
        gridCellBuffer.Release();

        deadBlocksBuffer.Release();
        deadCellsBuffer.Release();
        newCellsBuffer.Release();
        newBlockId.Release();

        attatchBlocksBuffer.Release();
        attatchBlocksRetrieveCountBuffer.Release();

        maxBlockBounds.Release();

        setVelocityBuffer.Release();
        playerAimDataBuffer.Release();
    }
}
