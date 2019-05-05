using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Block))]
public class BlockMover : MonoBehaviour
{
    public Vector2 velocity;

    private Block block;
    private Vector2 deltaPosition;

    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<Block>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaPosition += velocity * Time.deltaTime;
        if(deltaPosition.x >= 1)
        {
            block.Translate(1, 0);
            deltaPosition.x = 0;
        }
        else if (deltaPosition.x <= -1)
        {
            block.Translate(-1, 0);
            deltaPosition.x = 0;
        }
        if (deltaPosition.y >= 1)
        {
            block.Translate(0, 1);
            deltaPosition.y = 0;
        }
        else if (deltaPosition.y <= -1)
        {
            block.Translate(0, -1);
            deltaPosition.y = 0;
        }
    }
}
