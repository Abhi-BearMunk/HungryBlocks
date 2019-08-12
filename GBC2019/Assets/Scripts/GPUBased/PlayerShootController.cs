using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootController : MonoBehaviour
{
    public enum PowerUpType { Normal, Grenade, Whizzler};
    public PowerUpType currentPowerUp = PowerUpType.Normal;
    public string rHorizontal = "RHorizontal1";
    public string rVertical = "RVertical1";
    public string shoot = "Shoot1";
    public Block.CellSubType playerType;

    private float deadZone = 0.38f;
    private bool shotFired;
    GridComputeOperator gridOperator;
    [SerializeField]
    BlockProperties normal;
    [SerializeField]
    BlockProperties grenade;
    [SerializeField]
    BlockProperties whizzler;
    Dictionary<PowerUpType, BlockProperties> powerUpProperties = new Dictionary<PowerUpType, BlockProperties>();
    // Start is called before the first frame update
    void Start()
    {
        gridOperator = GetComponent<GridComputeOperator>();
        powerUpProperties.Add(PowerUpType.Normal, normal);
        powerUpProperties.Add(PowerUpType.Grenade, grenade);
        powerUpProperties.Add(PowerUpType.Whizzler, whizzler);
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
                BlockProperties properties = powerUpProperties[currentPowerUp];
                properties.velocityX = directionInt.x;
                properties.velocityY = directionInt.y;
                properties.subType = playerType;

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
