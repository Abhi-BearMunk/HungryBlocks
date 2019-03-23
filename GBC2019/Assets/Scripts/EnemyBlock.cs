using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlock : Blocks
{
    protected override void Update()
    {
        base.Update();
        if (gridPosX < -20 || gridPosX > LevelGrid.Instance().width + 20 || gridPosY < -20 || gridPosY > LevelGrid.Instance().height + 20)
        {
            Destroy(gameObject);
        }
    }
    protected override bool CheckBlocked(int x, int y)
    {
        foreach (Vector2Int key in myCells.Keys)
        {
            if (LevelGrid.Instance().GetGrid(gridPosX + key.x + x, gridPosY + key.y + y).GetGridType() == LevelGrid.GridType.Blocked)
            {
                return true;
            }
        }
        return false;
    }
}
