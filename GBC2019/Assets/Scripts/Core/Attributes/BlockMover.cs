using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Block))]
public class BlockMover : MonoBehaviour
{
    public float deltaTime = 0.8f;

    private Vector2Int velocity;
    private bool move = false;
    private float timer = 0;

    [HideInInspector]
    public Block block;
    [HideInInspector]
    public Vector2 deltaPosition;

    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<Block>();
    }

    //Update is called once per frame
    public void Update()
    {
        if(move)
        {
            timer -= Time.deltaTime;
            if(timer <= 0 && block.Translate(velocity))
            {
                timer = deltaTime * (Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y));
            }
        }
    }

    public void Translate(Vector2Int displacement)
    {
        if(displacement == velocity)
        {
            return;
        }
        velocity = displacement;
        if(displacement.sqrMagnitude == 0)
        {
            timer = 0;
        }
        move = displacement.sqrMagnitude == 0 ? false : true;
    }

    public void Translate(int x, int y)
    {
        Translate(new Vector2Int(x, y));
    }
}
