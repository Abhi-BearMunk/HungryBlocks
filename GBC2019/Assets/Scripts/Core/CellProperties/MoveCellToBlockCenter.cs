using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCellToBlockCenter : MonoBehaviour
{
    public Block block;

    Cell cell;
    // Start is called before the first frame update
    void Start()
    {
        cell = GetComponent<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cell)
        {
            cell.SetGridPosition(block.GetShape().aabbCenter);
        }
    }
}
