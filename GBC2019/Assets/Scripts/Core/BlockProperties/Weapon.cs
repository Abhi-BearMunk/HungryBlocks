using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class Weapon : MonoBehaviour
{
    public string absorbType = "P1Bullet";
    public float bulletDeltaTime = 0.02f;
    Block myBlock;
    GridManager grid;
    AbsorbData data;

    private void Start()
    {
        myBlock = GetComponent<Block>();
        data = GetComponent<AbsorbData>();
    }
    public void Shoot(Vector2Int direction)
    {
        CreateBullet(myBlock.GetShape().aabbCenter + new Vector2Int(direction.y, -direction.x), direction);
        CreateBullet(myBlock.GetShape().aabbCenter, direction);
        CreateBullet(myBlock.GetShape().aabbCenter - new Vector2Int(direction.y, -direction.x), direction);
    }

    public void CreateBullet(Vector2Int position, Vector2Int direction)
    {
        GameObject go;
        grid = myBlock.GetGrid();
        go = new GameObject("Bullet", typeof(Block), typeof(KillableByNonMatchingSubType), typeof(KillNonMatchingSubType), typeof(AbsorbData), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(PauseController));
        Block block = go.GetComponent<Block>();
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbData>().absorbType = absorbType;
        block.GetComponent<AbsorbData>().ignoreTypes.Add(data.absorbType);
        block.GetComponent<BlockMover>().deltaTime = bulletDeltaTime;
        block.GetComponent<BlockMover>().Translate(direction);
        block = grid.CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.Dot], position, Block.CellType.Enemy, myBlock.GetBlockSubType(), go);
    }
}
