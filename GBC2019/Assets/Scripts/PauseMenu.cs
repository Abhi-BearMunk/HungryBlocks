using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Continue"))
        {
            LevelManager.Instance().paused = false;
            LevelManager.Instance().levelAudio.UnPause();
            gameObject.SetActive(false);
        }

        if (Input.GetButtonDown("Restart"))
        {
            //LevelManager.Instance().DestryInstance();
            //LevelGrid.Instance().DestryInstance();
            //VictoryManager.Instance().DestryInstance();
            //BlockShapes.shapeDefinitions = null;
            SceneManager.LoadScene(0);
        }

        if (Input.GetButtonDown("Quit"))
        {
            //LevelManager.Instance().DestryInstance();
            //LevelGrid.Instance().DestryInstance();
            //VictoryManager.Instance().DestryInstance();
            //BlockShapes.shapeDefinitions = null;
            Application.Quit();
        }
    }
}
