using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class BlockShapes : MonoBehaviour
    {

        public enum BlockShape { Dot, Cross, Hline, Vline, Box2x2, H, Trident, UFO }
        public static Dictionary<BlockShape, bool[,]> shapeDefinitions = new Dictionary<BlockShape, bool[,]>();

        private void Awake()
        {
            shapeDefinitions.Clear();
            bool[,] shapeGrid;

            // Dot
            shapeGrid = new bool[1, 1];
            shapeGrid[0, 0] = true;
            shapeDefinitions.Add(BlockShape.Dot, shapeGrid);

            // Cross
            shapeGrid = new bool[3, 3];
            shapeGrid[0, 0] = false;
            shapeGrid[1, 0] = true;
            shapeGrid[2, 0] = false;
            shapeGrid[0, 1] = true;
            shapeGrid[1, 1] = true;
            shapeGrid[2, 1] = true;
            shapeGrid[0, 2] = false;
            shapeGrid[1, 2] = true;
            shapeGrid[2, 2] = false;
            shapeDefinitions.Add(BlockShape.Cross, shapeGrid);

            // Hline
            shapeGrid = new bool[3, 1];
            shapeGrid[0, 0] = true;
            shapeGrid[1, 0] = true;
            shapeGrid[2, 0] = true;
            shapeDefinitions.Add(BlockShape.Hline, shapeGrid);

            // Vline
            shapeGrid = new bool[1, 3];
            shapeGrid[0, 0] = true;
            shapeGrid[0, 1] = true;
            shapeGrid[0, 2] = true;
            shapeDefinitions.Add(BlockShape.Vline, shapeGrid);

            // Box2x2
            shapeGrid = new bool[2, 2];
            shapeGrid[0, 0] = true;
            shapeGrid[0, 1] = true;
            shapeGrid[1, 0] = true;
            shapeGrid[1, 1] = true;
            shapeDefinitions.Add(BlockShape.Box2x2, shapeGrid);

            // H
            shapeGrid = new bool[3, 3];
            shapeGrid[0, 0] = true;
            shapeGrid[0, 1] = true;
            shapeGrid[0, 2] = true;
            shapeGrid[1, 0] = false;
            shapeGrid[1, 1] = true;
            shapeGrid[1, 2] = false;
            shapeGrid[2, 0] = true;
            shapeGrid[2, 1] = true;
            shapeGrid[2, 2] = true;
            shapeDefinitions.Add(BlockShape.H, shapeGrid);

            // Trident
            shapeGrid = new bool[5, 4];
            shapeGrid[2, 0] = true;
            shapeGrid[2, 1] = true;
            shapeGrid[2, 2] = true;
            shapeGrid[2, 3] = true;
            shapeGrid[0, 2] = true;
            shapeGrid[1, 2] = true;
            shapeGrid[3, 2] = true;
            shapeGrid[4, 2] = true;
            shapeGrid[0, 3] = true;
            shapeGrid[4, 3] = true;

            shapeDefinitions.Add(BlockShape.Trident, shapeGrid);

            // UFO
            shapeGrid = new bool[7, 3];
            shapeGrid[0, 1] = true;
            shapeGrid[1, 1] = true;
            shapeGrid[2, 1] = true;
            shapeGrid[3, 1] = true;
            shapeGrid[4, 1] = true;
            shapeGrid[5, 1] = true;
            shapeGrid[6, 1] = true;

            shapeGrid[2, 0] = true;
            shapeGrid[3, 0] = true;
            shapeGrid[4, 0] = true;

            shapeGrid[2, 2] = true;
            shapeGrid[3, 2] = true;
            shapeGrid[4, 2] = true;


            shapeDefinitions.Add(BlockShape.UFO, shapeGrid);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
