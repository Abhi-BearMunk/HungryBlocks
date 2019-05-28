using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbControls : MonoBehaviour, IPausable
{
    public string yellow = "Yellow1";
    public string red = "Red1";
    public string blue = "Blue1";
    public string green = "Green1";

    PluckCellsInRange absorbCells;

    // Start is called before the first frame update
    void Start()
    {
        absorbCells = GetComponent<PluckCellsInRange>();
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if(Input.GetButtonDown(yellow) && absorbCells != null)
        {
            absorbCells.Absorb(Block.CellSubType.Y);
        }
        if (Input.GetButtonDown(red) && absorbCells != null)
        {
            absorbCells.Absorb(Block.CellSubType.R);
        }
        if (Input.GetButtonDown(blue) && absorbCells != null)
        {
            absorbCells.Absorb(Block.CellSubType.B);
        }
        if (Input.GetButtonDown(green) && absorbCells != null)
        {
            absorbCells.Absorb(Block.CellSubType.G);
        }
    }
}
