using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    [System.Serializable]
    public class GameObjectList
    {
        public List<GameObject> gameObjectList = new List<GameObject>();
    }

    public class LevelManager : MonoBehaviour
    {

        public List<GameObjectList> enemyPatternsByDifficulty = new List<GameObjectList>();

        public int difficulty = 0;
        public int waveNumber = 0;
        public int difficultyIncreasewaveMod = 10;

        public float waveTime = 5;
        public float minWaveTime = 2.5f;
        public float waveTimeDecreasePerWave = 0.1f;

        public float baseSpeed = 8;
        public float speedDifficultyModifier = 1;
        public float maxSpeed = 18;

        public bool paused;
        public GameObject pauseMenu;
        public AudioSource levelAudio;

        float timer;

        static LevelManager instance;


        public static LevelManager Instance()
        {
            return instance;
        }

        public void DestryInstance()
        {
            Destroy(instance);
            instance = null;
        }

        private void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!paused)
            {
                timer += Time.deltaTime;
                if (timer > waveTime && !VictoryManager.Instance().win)
                {
                    LaunchWave();
                    waveTime = Mathf.Max(waveTime - waveTimeDecreasePerWave, minWaveTime);
                    waveNumber++;
                    if (waveNumber % difficultyIncreasewaveMod == 0)
                    {
                        difficulty++;
                    }
                    timer = 0;
                }
            }
            if (Input.GetButtonDown("Pause"))
            {
                pauseMenu.SetActive(true);
                paused = true;
                levelAudio.Pause();
            }
        }

        void LaunchWave()
        {
            List<GameObject> gameObjectList = enemyPatternsByDifficulty[Random.Range(0, Mathf.Min(difficulty, enemyPatternsByDifficulty.Count - 1) + 1)].gameObjectList;
            GameObject waveObject = gameObjectList[Random.Range(0, gameObjectList.Count)];
            GameObject wave = Instantiate(waveObject);
            EnemyPattern pattern = wave.GetComponent<EnemyPattern>();

            float speed = Mathf.Min(baseSpeed + difficulty * speedDifficultyModifier, maxSpeed);

            foreach (EnemyBlock e in pattern.fixedGroup)
            {
                SetVelocity(e);
                e.defaultSpeed = speed;
                e.gameObject.SetActive(true);
            }

            List<LevelGrid.GridType> gridTypes = new List<LevelGrid.GridType>();
            gridTypes.Add(LevelGrid.GridType.R);
            gridTypes.Add(LevelGrid.GridType.G);
            gridTypes.Add(LevelGrid.GridType.B);
            gridTypes.Add(LevelGrid.GridType.Y);

            LevelGrid.GridType gridType;

            gridType = gridTypes[Random.Range(0, gridTypes.Count)];
            foreach (EnemyBlock e in pattern.group1)
            {
                SetVelocity(e);
                e.defaultSpeed = speed;
                e.myType = gridType;
                e.gameObject.SetActive(true);
            }
            gridTypes.Remove(gridType);

            gridType = gridTypes[Random.Range(0, gridTypes.Count)];
            foreach (EnemyBlock e in pattern.group2)
            {
                SetVelocity(e);
                e.defaultSpeed = speed;
                e.myType = gridType;
                e.gameObject.SetActive(true);
            }
            gridTypes.Remove(gridType);

            gridType = gridTypes[Random.Range(0, gridTypes.Count)];
            foreach (EnemyBlock e in pattern.group3)
            {
                SetVelocity(e);
                e.defaultSpeed = speed;
                e.myType = gridType;
                e.gameObject.SetActive(true);
            }
            gridTypes.Remove(gridType);

            gridType = gridTypes[Random.Range(0, gridTypes.Count)];
            foreach (EnemyBlock e in pattern.group4)
            {
                SetVelocity(e);
                e.defaultSpeed = speed;
                e.myType = gridType;
                e.gameObject.SetActive(true);
            }
        }

        void SetVelocity(EnemyBlock e)
        {
            if (e.gridPosX < 0)
            {
                e.velocity = new Vector2Int(1, 0);
            }
            else if (e.gridPosX > LevelGrid.Instance().width)
            {
                e.velocity = new Vector2Int(-1, 0);
            }
            else if (e.gridPosY < 0)
            {
                e.velocity = new Vector2Int(0, 1);
            }
            else if (e.gridPosY > LevelGrid.Instance().height)
            {
                e.velocity = new Vector2Int(0, -1);
            }
        }
    }
}
