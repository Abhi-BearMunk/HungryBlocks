using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class Blocks : MonoBehaviour
    {

        public LevelGrid.GridType myType;
        public GameObject cellPrefab;

        public int gridPosX = 0;
        public int gridPosY = 0;

        private int width = 1;
        private int height = 1;

        public bool[,] shape;

        public BlockShapes.BlockShape startingShape;

        public Dictionary<Vector2Int, Cell> myCells;

        public Vector2Int velocity = new Vector2Int(0, 0);
        public float defaultSpeed = 10;
        public float speedMultiplier = 1;
        float xInc = 0;
        float yInc = 0;

        public LevelGrid.GridType shieldType = LevelGrid.GridType.Empty;
        public int numberOfShots = 0;
        public int maxNumberOfShots = 5;
        public float megaComsumeTime = 1;
        public float megaTimer = 0;

        public AudioSource src;
        public AudioClip attach;
        public AudioClip absorb;


        // Use this for initialization
        void Start()
        {
            myCells = new Dictionary<Vector2Int, Cell>();
            CreateBlock();
        }

        public void CreateBlock()
        {
            if (myType != LevelGrid.GridType.Empty && myType != LevelGrid.GridType.Boundary)
            {
                shape = BlockShapes.shapeDefinitions[startingShape];
                width = shape.GetLength(0);
                height = shape.GetLength(1);
                GenerateCells();
                //MapCurrentShapeOnGrid();
                transform.position = new Vector3(gridPosX, gridPosY, -1) * LevelGrid.Instance().cellSize;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!LevelManager.Instance().paused)
            {
                CheckMovement();
                if (VictoryManager.Instance().win)
                {
                    velocity = Vector2Int.zero;
                }
                if (shieldType == LevelGrid.GridType.Mega)
                {
                    megaTimer += Time.deltaTime;
                    if (megaTimer > megaComsumeTime)
                    {
                        numberOfShots--;
                        DecreaseShieldGui();
                        megaTimer = 0;
                    }
                    if (numberOfShots == 0)
                    {
                        ChangeShield(myType);
                    }
                }
            }
        }

        protected virtual void DecreaseShieldGui()
        {

        }

        void GenerateCells()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (shape[i, j])
                    {
                        GameObject cell = Instantiate(cellPrefab, transform.position + new Vector3(i * LevelGrid.Instance().cellSize, j * LevelGrid.Instance().cellSize, 0), Quaternion.identity);
                        myCells.Add(new Vector2Int(i, j), cell.GetComponent<Cell>());
                        cell.GetComponent<Cell>().parentBlock = this;
                        cell.GetComponent<Cell>().SetColor();
                        SetBackgroundColor(cell.GetComponent<Cell>());
                        cell.transform.parent = transform;
                        LevelGrid.Instance().SetGrid(gridPosX + i, gridPosY + j, cell.GetComponent<Cell>());
                    }
                }
            }
        }

        protected virtual void SetBackgroundColor(Cell cell)
        {

        }

        protected void MapCurrentShapeOnGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (shape[i, j])
                    {
                        LevelGrid.Instance().SetGrid(gridPosX + i, gridPosY + j, myCells[new Vector2Int(i, j)]);
                    }
                }
            }
        }

        void CheckMovement()
        {
            if (velocity.magnitude > 0)
            {
                xInc += velocity.x * defaultSpeed * speedMultiplier * Time.deltaTime;
                yInc += velocity.y * defaultSpeed * speedMultiplier * Time.deltaTime;
                if (Mathf.Abs(xInc) >= 1)
                {
                    CheckMove((int)xInc, 0);
                    xInc = 0;
                }
                if (Mathf.Abs(yInc) >= 1)
                {
                    CheckMove(0, (int)yInc);
                    yInc = 0;
                }
            }
        }

        protected virtual bool CheckMove(int x, int y)
        {
            // Check for blocked
            if (CheckBlocked(x, y))
            {
                ChangeVelocityOnHit();
                return true;
            }
            // Check for other blocks I am colliding with
            List<Blocks> blocksToAbsorb = new List<Blocks>();
            List<Cell> cellsToRemove = new List<Cell>();
            foreach (Vector2Int key in myCells.Keys)
            {
                Cell cell = LevelGrid.Instance().GetGrid(gridPosX + key.x + x, gridPosY + key.y + y);

                // Otherwise for type mismatch
                if (cell.GetGridType() != myType && cell.GetGridType() != LevelGrid.GridType.Empty && cell.GetGridType() != LevelGrid.GridType.Blocked && cell.GetGridType() != LevelGrid.GridType.Boundary)
                {
                    bool shieldMatch1 = false;
                    // Remove my cell
                    if (shieldType == cell.GetGridType() && numberOfShots < maxNumberOfShots)
                    {
                        AddShot();
                        shieldMatch1 = true;
                    }
                    else if (shieldType != LevelGrid.GridType.Mega)
                    {
                        if (shieldType == cell.GetGridType() && numberOfShots == maxNumberOfShots)
                        {
                            OnMaxShield();
                        }
                        Cell myCell = myCells[new Vector2Int(key.x, key.y)];
                        cellsToRemove.Add(myCell);
                    }

                    bool shieldMatch2 = false;

                    if (cell.parentBlock.shieldType == myType && cell.parentBlock.numberOfShots < cell.parentBlock.maxNumberOfShots)
                    {
                        cell.parentBlock.AddShot();
                        shieldMatch2 = true;
                    }
                    else if (cell.parentBlock.shieldType != LevelGrid.GridType.Mega)
                    {
                        if (cell.parentBlock.shieldType == myType && cell.parentBlock.numberOfShots == cell.parentBlock.maxNumberOfShots)
                        {
                            cell.parentBlock.OnMaxShield();
                        }
                        // Remove enemy cell
                        cell.parentBlock.RemoveCell(cell);
                        //Destroy enemy cell
                        //Destroy(cell.gameObject);
                        cell.KillCell();
                    }
                    if (shieldMatch1 && shieldMatch2)
                    {
                        return false;
                    }
                }
                // If there is a type match
                else if (cell.GetGridType() == myType && cell.parentBlock != this)
                {
                    // If the other block is player,... sacrifice
                    try
                    {
                        var p = (PlayerBlock)(cell.parentBlock);

                        p.CellTransfer(this);
                        return false;
                    }
                    // Otherwise add to the list
                    catch
                    {
                        if (!blocksToAbsorb.Contains(cell.parentBlock))
                        {
                            blocksToAbsorb.Add(cell.parentBlock);
                        }
                    }
                }
            }

            // Add them cells
            foreach (Blocks block in blocksToAbsorb)
            {
                CellTransfer(block);
            }

            // Remove them cells
            foreach (Cell cell in cellsToRemove)
            {
                RemoveCell(cell);
                //Destroy(cell.gameObject);
                cell.KillCell();
            }

            foreach (Vector2Int key in myCells.Keys)
            {
                try
                {
                    LevelGrid.Instance().SetGrid(gridPosX + key.x, gridPosY + key.y, LevelGrid.Instance().emptyCells[gridPosX + key.x, gridPosY + key.y]);
                }
                catch
                {

                }
            }
            foreach (Vector2Int key in myCells.Keys)
            {
                LevelGrid.Instance().SetGrid(gridPosX + key.x + x, gridPosY + key.y + y, myCells[new Vector2Int(key.x, key.y)]);
            }

            gridPosX += x;
            gridPosY += y;
            transform.position += new Vector3(x, y, 0) * LevelGrid.Instance().cellSize;
            if (myCells.Count == 0)
            {
                OnDeath();
            }
            CheckWin();
            return true;
        }

        protected virtual void OnDeath()
        {

        }

        protected virtual void OnMaxShield()
        {

        }

        protected virtual bool CheckWin()
        {
            return false;
        }

        public void CellTransfer(Blocks block)
        {
            // Absorb
            foreach (Vector2Int key in block.myCells.Keys)
            {
                Cell cell = block.myCells[key];
                if (cell)
                {
                    cell.parentBlock = this;
                    cell.transform.parent = transform;
                    if (!myCells.ContainsKey(new Vector2Int(block.gridPosX - gridPosX, block.gridPosY - gridPosY) + key))
                    {
                        myCells.Add(new Vector2Int(block.gridPosX - gridPosX, block.gridPosY - gridPosY) + key, cell);
                        SetBackgroundColor(cell);
                    }
                }
            }
            block.myCells = new Dictionary<Vector2Int, Cell>();
            Destroy(block.gameObject);
            if (src && attach)
            {
                src.PlayOneShot(attach);
            }
        }

        public virtual void AddShot()
        {
            numberOfShots++;
            if (src && absorb)
            {
                src.PlayOneShot(absorb);
            }
        }

        protected virtual bool CheckBlocked(int x, int y)
        {
            return false;
        }

        protected virtual void ChangeVelocityOnHit()
        {

        }

        public void AddCell(Cell cell, int i, int j)
        {
            cell.parentBlock = this;
            cell.transform.parent = transform;
            if (!myCells.ContainsKey(new Vector2Int(i, j)))
            {
                myCells.Add(new Vector2Int(i, j), cell);
            }
            else
            {
                Destroy(cell.gameObject);
            }
        }

        public bool RemoveCell(Cell cell)
        {
            foreach (Vector2Int key in myCells.Keys)
            {
                if (myCells[key] == cell)
                {
                    myCells[key].parentBlock = null;
                    myCells.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public virtual void Kill()
        {

        }

        public void ForceStop()
        {
            xInc = 0;
            yInc = 0;
        }

        public void ForceMove(Vector2Int vel)
        {
            xInc = vel.x;
            yInc = vel.y;
        }

        public void ChangeShield(LevelGrid.GridType gridType)
        {
            shieldType = gridType;
            foreach (Cell cell in myCells.Values)
            {
                SetBackgroundColor(cell);
            }
        }

    }
}
