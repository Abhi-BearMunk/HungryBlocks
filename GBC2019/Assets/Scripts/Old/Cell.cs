using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class Cell : MonoBehaviour
    {

        public Blocks parentBlock;
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer spriteBackground;
        public CellAnimation cellAnim;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetColor()
        {
            if (spriteRenderer && parentBlock)
            {
                spriteRenderer.color = LevelGrid.Instance().typeColor[parentBlock.myType];
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if (spriteBackground && parentBlock)
            {
                Color alphaColor = color;
                alphaColor.a = 0.5f;
                spriteBackground.color = alphaColor;
            }
        }

        public void ToggleShieldVisual(bool toggle)
        {
            spriteBackground.gameObject.SetActive(toggle);
        }

        public LevelGrid.GridType GetGridType()
        {
            if (!parentBlock)
            {
                Debug.Log("No parent block " + name);
                return LevelGrid.GridType.Empty;
                //Destroy(gameObject);
            }
            return parentBlock.myType;
        }

        public void KillCell()
        {
            if (cellAnim)
            {
                cellAnim.enabled = true;
                cellAnim.Disintigrate();
            }
            else
            {
                Destroy(gameObject);
            }
            Destroy(this);
        }
    }
}
