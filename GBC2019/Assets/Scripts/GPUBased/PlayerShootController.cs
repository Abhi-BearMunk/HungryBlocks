using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    public string rHorizontal = "RHorizontal1";
    public string rVertical = "RVertical1";
    public string shoot = "Shoot1";
    public Block.CellSubType playerType;

    private float deadZone = 0.38f;
    private bool shotFired;
    GridComputeOperator gridOperator;
    // Start is called before the first frame update
    void Start()
    {
        gridOperator = GetComponent<GridComputeOperator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxis(rHorizontal), Input.GetAxis(rVertical));
        direction.Normalize();
        Vector2Int directionInt = new Vector2Int(Mathf.Abs(direction.x) > deadZone ? (int)Mathf.Sign(direction.x) : 0, Mathf.Abs(direction.y) > deadZone ? (int)Mathf.Sign(direction.y) : 0);
        gridOperator.player1AimDirection.x = directionInt.x;
        gridOperator.player1AimDirection.y = directionInt.y;

        if (directionInt.sqrMagnitude > 0)
        {
            // Shoot
            if (Input.GetAxis(shoot) > deadZone && !shotFired)
            {
                BlockProperties properties;
                properties.moveTicks = 1;
                properties.velocityX = directionInt.x;
                properties.velocityY = directionInt.y;
                properties.type = Block.CellType.Enemy;
                properties.subType = playerType;
                properties.absorbPriority = 0;
                properties.absorbType = 2;
                properties.ignoreType = 1;
                properties.canAbsorb = 0;
                properties.CanBeAbsorbed = 1;
                properties.KillNonMatching = 1;
                properties.KillableByNonMatching = 1;
                properties.isGrenade = 1;

                gridOperator.CreateBlock(ShapeDictionary.shapeDefinitions[ShapeDictionary.BlockShape.Dot], new Vector2Int(0, 0), properties, gridOperator.player1ID);
                shotFired = true;
            }
            else if (Input.GetAxis(shoot) <= deadZone)
            {
                shotFired = false;
            }
        }
    }
}
