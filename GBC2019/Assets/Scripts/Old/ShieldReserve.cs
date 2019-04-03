using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class ShieldReserve : MonoBehaviour
    {
        public List<SpriteRenderer> shotUI = new List<SpriteRenderer>();
        public Color fillColor;
        public Color emptyColor;
        public Animator anim;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateUI(int index, bool toggle)
        {
            shotUI[index].color = toggle ? fillColor : emptyColor;
        }

        public void Shake()
        {
            anim.SetTrigger("Shake");
        }
    }
}
