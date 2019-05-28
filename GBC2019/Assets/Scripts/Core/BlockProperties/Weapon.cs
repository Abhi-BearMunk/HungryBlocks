using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbsorbData))]
public class Weapon : MonoBehaviour, IRegisterProperty, IWeapon
{
    public string absorbType = "P1Bullet";
    public float bulletDeltaTime = 0.02f;
    Block myBlock;
    GridManager grid;

    public void Register(Block block)
    {
        myBlock = block;
    }
    public void Shoot(Vector2Int direction)
    {
        //CreateBullet(myBlock.GetShape().aabbCenter + new Vector2Int(direction.y, -direction.x), direction);
        CreateBullet(myBlock.GetShape().aabbCenter, direction);
        //CreateBullet(myBlock.GetShape().aabbCenter - new Vector2Int(direction.y, -direction.x), direction);
    }

    public void CreateBullet(Vector2Int position, Vector2Int direction)
    {
        GameObject go = new GameObject("Bullet", typeof(Block), typeof(KillNonMatchingSubType), typeof(KillableByNonMatchingSubType), typeof(AbsorbData), typeof(AbsorbableByMatchingSubType), typeof(DestroyOnKill), typeof(PauseController));
        Block block = go.GetComponent<Block>();
        block.gameObject.AddComponent<BlockMover>();
        block.GetComponent<AbsorbData>().absorbType = absorbType;
        block.GetComponent<AbsorbData>().ignoreTypes.Add(myBlock.absorbData.absorbType);
        block.GetComponent<BlockMover>().deltaTime = bulletDeltaTime;
        block.GetComponent<BlockMover>().Translate(direction);
        block = myBlock.GetGrid().CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.Dot], position, Block.CellType.Enemy, myBlock.GetBlockSubType(), go);
    }
}
