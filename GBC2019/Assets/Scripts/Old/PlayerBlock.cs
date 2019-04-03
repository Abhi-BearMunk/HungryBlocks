using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class PlayerBlock : Blocks
    {
        Vector2Int bottomLeft = new Vector2Int(0, 0);
        Vector2Int center = new Vector2Int(0, 0);
        public ShieldReserve shieldReserve;
        public GameObject enemyBulletBlock;
        public float bulletSpeed = 20;
        public bool playWin = false;
        public float winLerpSpeed = 10f;
        public float winScale = 2;
        public float winDelay = 0.5f;

        int lastSize;
        Vector2 centerPoint = new Vector2(0, 0);
        Vector3 relativeScreenCentre;

        public AudioClip shoot;
        public AudioClip maxShield;


        public TextMesh countdownText;
        public float countdownTime = 10;
        public float countdownTimer = 0;

        bool dead = false;

        protected override void Update()
        {
            base.Update();

            if (myCells.Count != lastSize && myCells.Count > 0)
            {
                centerPoint = Vector2.zero;
                foreach (Vector2Int key in myCells.Keys)
                {
                    centerPoint += new Vector2(key.x, key.y);
                }
                centerPoint /= myCells.Keys.Count;
                center = new Vector2Int((int)Mathf.Round(centerPoint.x), (int)Mathf.Round(centerPoint.y));
                if (shieldReserve)
                {
                    shieldReserve.transform.position = (Vector3)centerPoint + new Vector3(gridPosX, gridPosY, -2);
                }
            }

            if (playWin)
            {

                transform.position = Vector3.Lerp(transform.position, relativeScreenCentre, winLerpSpeed * Time.deltaTime);
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(winScale, winScale, winScale), winLerpSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, relativeScreenCentre) < 1 && !VictoryManager.Instance().winUI.activeSelf)
                {
                    VictoryManager.Instance().winUI.SetActive(true);
                }
            }

            if (myCells.Count == 0)
            {
                OnDeath();
            }

            lastSize = myCells.Count;
        }

        protected override bool CheckBlocked(int x, int y)
        {
            if (myCells.Count == 0)
            {
                return LevelGrid.Instance().OutOfBounds(gridPosX + x, gridPosY + y);
            }
            foreach (Vector2Int key in myCells.Keys)
            {
                if (LevelGrid.Instance().GetGrid(gridPosX + key.x + x, gridPosY + key.y + y).GetGridType() == LevelGrid.GridType.Blocked
                  || LevelGrid.Instance().GetGrid(gridPosX + key.x + x, gridPosY + key.y + y).GetGridType() == LevelGrid.GridType.Boundary)
                {
                    return true;
                }
            }
            return false;
        }
        protected override void SetBackgroundColor(Cell cell)
        {
            base.SetBackgroundColor(cell);
            if (shieldType == LevelGrid.GridType.Empty)
            {
                cell.SetBackgroundColor(LevelGrid.Instance().typeColor[myType]);
            }
            else
            {
                cell.SetBackgroundColor(LevelGrid.Instance().typeColor[shieldType]);
            }

        }

        public override void AddShot()
        {
            shieldReserve.UpdateUI(numberOfShots, true);
            base.AddShot();

        }

        protected override void OnMaxShield()
        {
            base.OnMaxShield();
            shieldReserve.Shake();
            if (src && maxShield)
            {
                src.PlayOneShot(maxShield);
            }
        }

        public void Shoot(Vector2Int direction)
        {

            if (numberOfShots <= 0)
            {
                return;
            }
            Cell cell = LevelGrid.Instance().GetGrid(center.x + gridPosX, center.y + gridPosY);
            int i = center.x + gridPosX;
            int j = center.y + gridPosY;

            while (cell && cell.GetGridType() != LevelGrid.GridType.Empty)
            {
                if (cell.parentBlock != this || cell.GetGridType() == LevelGrid.GridType.Blocked || cell.GetGridType() == LevelGrid.GridType.Boundary)
                {
                    return;
                }
                i += direction.x;
                j += direction.y;
                cell = LevelGrid.Instance().GetGrid(i, j);
            }



            EnemyBlock enemy = Instantiate(enemyBulletBlock).GetComponent<EnemyBlock>();
            enemy.transform.parent = null;
            enemy.myType = shieldType;
            enemy.gridPosX = i;
            enemy.gridPosY = j;
            enemy.velocity = direction;
            enemy.defaultSpeed = bulletSpeed;
            enemy.gameObject.SetActive(true);
            numberOfShots--;
            shieldReserve.UpdateUI(numberOfShots, false);
            if (src && shoot)
            {
                src.PlayOneShot(shoot);
            }
        }

        protected override bool CheckWin()
        {
            if (myCells.Count == VictoryManager.Instance().winningPoints.Count)
            {
                foreach (Vector2Int key in myCells.Keys)
                {
                    if (!VictoryManager.Instance().winningPoints.Contains(key + new Vector2Int(gridPosX, gridPosY)))
                    {
                        return false;
                    }

                }
                relativeScreenCentre = new Vector3((float)LevelGrid.Instance().width / 2 - (centerPoint.x + 0.5f) * winScale, (float)LevelGrid.Instance().height / 2 - (centerPoint.y + 0.5f) * winScale, -9);
                VictoryManager.Instance().Win();
                Invoke("PlayWin", winDelay);
                return true;
            }
            return base.CheckWin();
        }

        void PlayWin()
        {
            playWin = true;
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            if (!dead)
            {
                dead = true;
                StartCoroutine("StartCountDown");
            }
        }


        IEnumerator StartCountDown()
        {
            countdownTimer = countdownTime;
            countdownText.text = countdownTimer.ToString();
            countdownText.gameObject.SetActive(true);

            while (countdownTimer > 0 && !LevelManager.Instance().paused)
            {
                yield return new WaitForSeconds(1);
                countdownTimer--;
                countdownText.text = countdownTimer.ToString();
            }
            countdownText.gameObject.SetActive(false);
            CreateBlock();
            dead = false;
            yield return null;
        }

        protected override void DecreaseShieldGui()
        {
            base.DecreaseShieldGui();
            shieldReserve.UpdateUI(numberOfShots, false);

        }
    }
}
