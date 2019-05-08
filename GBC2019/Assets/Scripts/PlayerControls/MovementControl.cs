using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockMover))]
public class MovementControl : MonoBehaviour, IPausable
{
    public BlockMover block;
    public float speed = 10;
    public float deadZone = 0.38f;
    public string lHorizontal = "LHorizontal1";
    public string lVertical = "LVertical1";
    public string rotateLeft = "RotateRight1";
    public string rotateRight = "RotateLeft1";

    Vector2 lastVel;
    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<BlockMover>();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        Vector2 move = new Vector2(Input.GetAxis(lHorizontal), Input.GetAxis(lVertical));
        //move.Normalize();
        Vector2Int moveInt = new Vector2Int(Mathf.Abs(move.x) > deadZone ? (int)Mathf.Sign(move.x) : 0, Mathf.Abs(move.y) > deadZone ? (int)Mathf.Sign(move.y) : 0);
        block.Translate(moveInt);
        if(Input.GetButtonDown(rotateRight))
        {
            while(!GetComponent<Block>().Rotate(1))
            {

            }
        }
        if (Input.GetButtonDown(rotateLeft))
        {
            while (!GetComponent<Block>().Rotate(-1))
            {

            }
        }
    }
}
