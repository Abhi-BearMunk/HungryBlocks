using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace old
{
    public class CellAnimation : MonoBehaviour
    {

        public Animator anim;
        bool disintegrate;
        public float speed = 10;
        Vector3 rand;

        public AudioClip destroy;
        public AudioSource src;
        // Use this for initialization
        void Start()
        {
            rand = Random.insideUnitCircle.normalized;
        }

        // Update is called once per frame
        void Update()
        {
            if (disintegrate)
            {
                transform.position += rand * speed;
            }
        }

        public void Disintigrate()
        {
            if (src && destroy)
            {
                src.PlayOneShot(destroy);
            }
            disintegrate = true;
            anim.SetBool("Kill", true);
        }

        public void DestroyCell()
        {
            Destroy(gameObject);
        }
    }
}
