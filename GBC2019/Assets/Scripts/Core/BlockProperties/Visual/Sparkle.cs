using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkle : MonoBehaviour
{
    public float sparkleTime = 0.12f;
    float timer;
    Block block;
    Cell current = null;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        timer = sparkleTime;
        block = GetComponent<Block>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= sparkleTime)
        {
            counter++;
            counter = counter % block.GetShape().cellList.Count;
            if (current != null)
            {
                current.GetComponent<CellVisualizer>().cellGlow.enabled = false;
            }
            if(block.GetShape().cellList.Count > 0)
            {
                current = block.GetShape().cellList[counter];
                current.GetComponent<CellVisualizer>().cellGlow.enabled = true;
            }           
            timer = 0;
        }
    }
}
