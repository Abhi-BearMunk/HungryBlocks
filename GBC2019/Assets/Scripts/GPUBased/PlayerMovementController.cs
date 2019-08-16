using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public int playerId;
    public float speed = 10;
    public float deadZone = 0.38f;
    public string lHorizontal = "LHorizontal1";
    public string lVertical = "LVertical1";
    public string rotateLeft = "RotateRight1";
    public string rotateRight = "RotateLeft1";
    GridComputeOperator gridOperator;
    Vector2Int lastMove;
    int rotationDirection;
    // Start is called before the first frame update
    void Start()
    {
        gridOperator = GetComponent<GridComputeOperator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector2(Input.GetAxis(lHorizontal), Input.GetAxis(lVertical));
        //move.Normalize();
        Vector2Int moveInt = new Vector2Int(Mathf.Abs(move.x) > deadZone ? (int)Mathf.Sign(move.x) : 0, Mathf.Abs(move.y) > deadZone ? (int)Mathf.Sign(move.y) : 0);

        if (Input.GetButtonDown(rotateRight))
        {
            rotationDirection = 1;
        }
        if (Input.GetButtonDown(rotateLeft))
        {
            rotationDirection = -1;
        }

        if (moveInt != lastMove)
        {
            gridOperator.SetVelocity(playerId, moveInt.x, moveInt.y);
        }

        if (rotationDirection != 0)
        {
            gridOperator.SetRotation(playerId, rotationDirection);
        }

        lastMove = moveInt;
        rotationDirection = 0;
    }
}
