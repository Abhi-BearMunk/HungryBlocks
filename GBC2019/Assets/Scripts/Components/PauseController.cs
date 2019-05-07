using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    LevelManager levelManager;
    IPausable[] pausables;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        pausables = GetComponents<IPausable>();
    }

    // Update is called once per frame
    void Update()
    {
        if(levelManager && !levelManager.paused)
        {
            foreach(IPausable pausable in pausables)
            {
                pausable.OnUpdate();
            }
        }
    }
}
