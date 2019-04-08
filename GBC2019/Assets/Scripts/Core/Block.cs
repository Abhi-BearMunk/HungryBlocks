using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Cell.CellType blockType;
    public GridManager grid;
    public Shape shape { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewBlockShape(Shape _shape)
    {
        shape = _shape;
    }

    public void SetBlockType(Cell.CellType type)
    {
        blockType = type;
    }

    public Cell.CellType GetBlockType()
    {
       return blockType;
    }

    private void UpdatePositionUsingShapeAABB()
    {
        transform.position = shape.aabbCenterAbsolute;
    }
}
