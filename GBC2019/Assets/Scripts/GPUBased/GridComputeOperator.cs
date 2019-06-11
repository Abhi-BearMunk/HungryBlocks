using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockProperties
{
    public Block.CellType type;
    public Block.CellSubType subType;
    public int velocityX;
    public int velocityY;
    public int moveTicks;
    public int absorbPriority;
    public int absorbType;
    public int ignoreType;
    public int canAbsorb;
    public int CanBeAbsorbed;
    public int KillNonMatching;
    public int KillableByNonMatching;
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

    public static int GetLength()
    {
        return sizeof(int) * 25;
    }
}

public struct CellStruct
{
    public int ID;
    public int gridPosX;
    public int gridPosY;
    public int parentBlockID;
    public int dying;
    public int dead;
    public int left;
    public int right;
    public int up;
    public int down;
    public int rotTicks;
    public int lastX;
    public int lastY;

    public static int GetLength()
    {
        return sizeof(int) * 13;
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

public struct int2
{
    public int x;
    public int y;

    public static int GetLength()
    {
        return sizeof(int) * 2;
    }
}

public class GridComputeOperator : MonoBehaviour
{
    public int width = 128;
    public int height = 72;
    public int scalingFactor = 20;
    public Texture2D cellSprite;
    public Renderer rend;
    public Renderer debugRend;
    public RenderTexture displayTexture;
    public RenderTexture debugTexture;
    public ComputeShader gridCruncher;

    BlockStruct[] blockArray;
    CellStruct[] cellArray;
    GridCell[] grid;

    ComputeBuffer blockBuffer;
    ComputeBuffer cellBuffer;
    ComputeBuffer gridCellBuffer;

    ComputeBuffer deadBlocksBuffer;
    ComputeBuffer deadCellsBuffer;
    ComputeBuffer newCellsBuffer;

    ComputeBuffer attatchBlocksBuffer;
    ComputeBuffer attatchBlocksRetrieveCountBuffer;

    int creationId = 0;
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize arrays
        blockArray = new BlockStruct[width * height];
        cellArray = new CellStruct[width * height];
        grid = new GridCell[width * height];

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


        // Set Buffers
        blockBuffer.SetData(blockArray);
        cellBuffer.SetData(cellArray);
        gridCellBuffer.SetData(grid);

        // Create and set render
        displayTexture = new RenderTexture(width * scalingFactor, height * scalingFactor, 24);
        displayTexture.enableRandomWrite = true;
        displayTexture.format = RenderTextureFormat.ARGB64;
        displayTexture.antiAliasing = 8;
        displayTexture.filterMode = FilterMode.Trilinear;
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


        //CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.SkullB], new Vector2Int(20, 20), Block.CellType.Enemy, Block.CellSubType.Y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int kernel = gridCruncher.FindKernel("TickBlock");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 16, 1, 1);

        attatchBlocksBuffer.SetCounterValue(0);

        kernel = gridCruncher.FindKernel("CheckBlockCollisions");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.SetBuffer(kernel, "attatchBlocksBuffer", attatchBlocksBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);

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
            int multipleOf8Offset = 8 - (attatchBlocks.Length % 8);
            multipleOf8Offset = multipleOf8Offset == 8 ? 0 : multipleOf8Offset;

            int2[] attatchBlocksCurated = new int2[attatchBlocks.Length + multipleOf8Offset];
            for (int i = 0; i < attatchBlocks.Length; i++)
            {
                attatchBlocksCurated[i].x = attatchBlocks[i].x;
                attatchBlocksCurated[i].y = attatchBlocksDictionary[attatchBlocks[i].x];
            }

            for (int i = 0; i < multipleOf8Offset; i++)
            {
                attatchBlocksCurated[attatchBlocksCurated.Length - i - 1].x = -1;
                attatchBlocksCurated[attatchBlocksCurated.Length - i - 1].y = -1;
            }
            //attatchBlocksBuffer.SetCounterValue(0);
            //attatchBlocksSetDataBuffer.Release();
            //attatchBlocksSetDataBuffer = new ComputeBuffer(attatchBlocksCurated.Length, sizeof(int) * 2, ComputeBufferType.Default);
            attatchBlocksBuffer.SetData(attatchBlocksCurated);

            kernel = gridCruncher.FindKernel("UpdateAttatchBlockID");
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
            gridCruncher.SetBuffer(kernel, "attatchBlocksBufferCurated", attatchBlocksBuffer);
            gridCruncher.Dispatch(kernel, attatchBlocksCurated.Length / 8, 1, 1);

            kernel = gridCruncher.FindKernel("AttatchBlockCells");
            gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 8, 1, 1);
        }


        kernel = gridCruncher.FindKernel("MoveCells");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);

        RefreshGrid();

        kernel = gridCruncher.FindKernel("PostMoveKillNonMatchingCells");
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);

        RefreshGrid();

        kernel = gridCruncher.FindKernel("ResetBlockDeathAndMove");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 16, 1, 1);

        kernel = gridCruncher.FindKernel("RefreshBlockDeath");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);


        kernel = gridCruncher.FindKernel("FlushBlockBounds");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 16, 1, 1);

        kernel = gridCruncher.FindKernel("UpdateBlockBounds");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);

        kernel = gridCruncher.FindKernel("UpdateBlockCenters");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 16, 1, 1);

        kernel = gridCruncher.FindKernel("ClearDisplay");
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.Dispatch(kernel, (width * scalingFactor) / 16, (height * scalingFactor) / 9, 1);

        kernel = gridCruncher.FindKernel("DisplayCells");
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.SetTexture(kernel, "cellSprite", cellSprite);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, (width * scalingFactor) / 16, (height * scalingFactor) / 9, 1);

        kernel = gridCruncher.FindKernel("DisplayDyingCells");
        gridCruncher.SetTexture(kernel, "Result", displayTexture);
        gridCruncher.SetTexture(kernel, "cellSprite", cellSprite);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.Dispatch(kernel, (scalingFactor) / 10, (scalingFactor) / 10, cellArray.Length / 8);

        kernel = gridCruncher.FindKernel("LerpCells");
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);
    }

    public void CreateBlock(List<Vector2Int> shapeDefinition, Vector2Int position, BlockProperties properties)
    {
        creationId++;
        // Copy shape onto a int2 array
        int paddingToMakeItMultipleOf8 = 8 - (shapeDefinition.Count % 8);
        int2[] shape = new int2[shapeDefinition.Count + paddingToMakeItMultipleOf8];
        int index = 0;
        foreach(Vector2Int v in shapeDefinition)
        {
            shape[index].x = v.x + position.x;
            shape[index].y = v.y + position.y;
            index++;
        }

        for (int i = index; i < shape.Length; i++)
        {
            shape[i].x = -10000;
            shape[i].y = -10000;
        }

        // Set New Parameters
        gridCruncher.SetInt("newCreationID", creationId);
        gridCruncher.SetInt("newType", (int)properties.type);
        gridCruncher.SetInt("newSubType", (int)properties.subType);
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

        // Reset the new cell buffer
        newCellsBuffer.SetData(shape);

        // Flush deadCells and deadBlocks Buffer
        deadBlocksBuffer.SetCounterValue(0);
        deadCellsBuffer.SetCounterValue(0);

        // Find dead blocks
        int kernel = gridCruncher.FindKernel("GetDeadBlocks");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksBuffer", deadBlocksBuffer);
        gridCruncher.Dispatch(kernel, blockArray.Length / 16, 1, 1);

        // Assign properties of the new block
        kernel = gridCruncher.FindKernel("AssignBlockProperties");
        gridCruncher.SetBuffer(kernel, "blockBuffer", blockBuffer);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.Dispatch(kernel, 1, 1, 1);

        // Get all the dead cells
        kernel = gridCruncher.FindKernel("GetDeadCells");
        gridCruncher.SetBuffer(kernel, "deadCellsBuffer", deadCellsBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);

        // Add the new cells and set their properties
        kernel = gridCruncher.FindKernel("AddCells");
        gridCruncher.SetBuffer(kernel, "newCells", newCellsBuffer);
        gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
        //deadCellsBuffer.SetCounterValue((uint)shape.Length);
        gridCruncher.SetBuffer(kernel, "deadBlocksCurated", deadBlocksBuffer);
        gridCruncher.SetBuffer(kernel, "deadCellsCurated", deadCellsBuffer);
        gridCruncher.Dispatch(kernel, shape.Length / 8, 1, 1);

        RefreshGrid();
    }

    void RefreshGrid()
    {
        int kernel = gridCruncher.FindKernel("FlushGridData");
        gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
        gridCruncher.Dispatch(kernel, width / 16, height / 9, 1);

        for(int i = 0; i < 10; i++)
        {
            gridCruncher.SetInt("gridCellIndex", i);
            kernel = gridCruncher.FindKernel("UpdateGridDataAtIndex");
            gridCruncher.SetBuffer(kernel, "grid", gridCellBuffer);
            gridCruncher.SetBuffer(kernel, "cellBuffer", cellBuffer);
            gridCruncher.Dispatch(kernel, cellArray.Length / 16, 1, 1);
        }
    }

    void OnDestroy()

    {
        blockBuffer.Release();
        cellBuffer.Release();
        gridCellBuffer.Release();
        newCellsBuffer.Release();
        attatchBlocksBuffer.Release();
        attatchBlocksRetrieveCountBuffer.Release();
        deadBlocksBuffer.Release();
        deadCellsBuffer.Release();
    }
}
