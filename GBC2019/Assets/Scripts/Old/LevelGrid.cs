using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class LevelGrid : MonoBehaviour
    {

        public enum GridType { Empty, R, G, B, Y, Blocked, Boundary, Mega }
        public Dictionary<GridType, Color> typeColor = new Dictionary<GridType, Color>();
        public int width = 10;
        public int height = 20;
        public float cellSize = 1;
        Cell[,] levelGrid;
        public Cell[,] emptyCells;
        public Cell emptyCell;

        public Cell boundary;
        public Blocks levelBlock;

        public GameObject emptyCellPrefab;

        static LevelGrid instance;

        private void Awake()
        {
            instance = this;
            levelGrid = new Cell[width, height];
            emptyCells = new Cell[width, height];
            InitializeGridCellObjects();
            typeColor.Add(GridType.Empty, Color.white);
            typeColor.Add(GridType.R, Color.red);
            typeColor.Add(GridType.G, Color.green);
            typeColor.Add(GridType.B, Color.blue);
            typeColor.Add(GridType.Y, Color.yellow);
            typeColor.Add(GridType.Blocked, Color.black);
            typeColor.Add(GridType.Boundary, Color.black);
            typeColor.Add(GridType.Mega, Color.white);
        }

        public static LevelGrid Instance()
        {
            return instance;
        }

        public void DestryInstance()
        {
            Destroy(instance);
            instance = null;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void InitializeGridCellObjects()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject emptyCell = Instantiate(emptyCellPrefab, transform.position + new Vector3(i * LevelGrid.Instance().cellSize, j * LevelGrid.Instance().cellSize, 0), Quaternion.identity);
                    levelGrid[i, j] = emptyCell.GetComponent<Cell>();
                    levelGrid[i, j].parentBlock = levelBlock;
                    emptyCells[i, j] = emptyCell.GetComponent<Cell>();
                }
            }

            GameObject emptyCellO = Instantiate(emptyCellPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            emptyCell = emptyCellO.GetComponent<Cell>();
            emptyCell.parentBlock = levelBlock; 
        }

        public Cell GetGrid(int i, int j)
        {
            try
            {
                if (!levelGrid[i, j])
                {
                    levelGrid[i, j] = emptyCell;
                }
                return levelGrid[i, j];
            }
            catch
            {
                return boundary;
            }
        }

        public void SetGrid(int i, int j, Cell cell)
        {
            if (cell)
            {
                try
                {
                    levelGrid[i, j] = cell;
                }
                catch
                {
                    Debug.LogWarning("Grid Type " + cell.name + " assign attempt at + " + i + ", " + j + " out of bounds");
                }
            }
        }

        public bool OutOfBounds(int i, int j)
        {
            return i < 0 || i >= width || j < 0 || j >= height;
        }
    }
}
