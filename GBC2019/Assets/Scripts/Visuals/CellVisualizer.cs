using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <summary>
// Maintains the visuals for a given Cell
// </summary>
public class CellVisualizer : MonoBehaviour
{
    public GridManager grid;
    public Cell cell;
    public float scaleFactor = 100 / 256;
    public List<Sprite> RSprites = new List<Sprite>();
    public List<Sprite> GSprites = new List<Sprite>();
    public List<Sprite> BSprites = new List<Sprite>();
    public List<Sprite> YSprites = new List<Sprite>();
    public Dictionary<Cell.CellType, List<Sprite>> cellSprites = new Dictionary<Cell.CellType, List<Sprite>>();

    // Graphic Objects
    public SpriteRenderer cellBase;
    public GameObject arrow;
    // Start is called before the first frame update
    void Awake()
    {
        cellSprites.Add(Cell.CellType.R, RSprites);
        cellSprites.Add(Cell.CellType.G, GSprites);
        cellSprites.Add(Cell.CellType.B, BSprites);
        cellSprites.Add(Cell.CellType.Y, YSprites);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGrid(GridManager _grid)
    {
        grid = _grid;
    }

    public void SetCellType(Cell.CellType cellType)
    {

    }
}
