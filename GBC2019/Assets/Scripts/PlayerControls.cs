using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public PlayerBlock playerBlock;
    public float deadZone = 0.1f;

    public string lHorizontal;
    public string lVertical;
    public string RShield;
    public string GShield;
    public string BShield;
    public string YShield;
    public string megaShield;
    public string rHorizontal;
    public string rVertical;
    public string shoot;

    public bool useKeyboard;

    Vector2Int lastVel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!LevelManager.Instance().paused)
        {
            Move();
            ChangeShield();
            Shoot();
            if (VictoryManager.Instance().win)
            {
                this.enabled = false;
            }
        }           
	}

    void Move()
    {
        Vector2 move = new Vector2(Input.GetAxis(lHorizontal), Input.GetAxis(lVertical));
        if(useKeyboard && !Input.GetKey(KeyCode.LeftShift))
        {
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        if(Mathf.Abs(move.x) > Mathf.Abs(move.y) && Mathf.Abs(move.x) > deadZone)
        {
            playerBlock.velocity = new Vector2Int((int)Mathf.Sign(move.x), 0);
            if(playerBlock.velocity != lastVel)
            {
                playerBlock.ForceMove(playerBlock.velocity);
            }
        }
        else if(Mathf.Abs(move.y) > deadZone)
        {
            playerBlock.velocity = new Vector2Int(0, (int)Mathf.Sign(move.y));
            if (playerBlock.velocity != lastVel)
            {
                playerBlock.ForceMove(playerBlock.velocity);
            }
        }
        else
        {
            playerBlock.velocity = Vector2Int.zero;
            playerBlock.ForceStop();
        }

        lastVel = playerBlock.velocity;
    }

    void ChangeShield()
    {
        if (Input.GetButtonDown(RShield) || (useKeyboard && Input.GetKeyDown(KeyCode.Alpha1)))
        {
            playerBlock.ChangeShield(LevelGrid.GridType.R);
        }
        if (Input.GetButtonDown(GShield) || (useKeyboard && Input.GetKeyDown(KeyCode.Alpha2)))
        {
            playerBlock.ChangeShield(LevelGrid.GridType.G);
        }
        if (Input.GetButtonDown(BShield) || (useKeyboard && Input.GetKeyDown(KeyCode.Alpha3)))
        {
            playerBlock.ChangeShield(LevelGrid.GridType.B);
        }
        if (Input.GetButtonDown(YShield) || (useKeyboard && Input.GetKeyDown(KeyCode.Alpha4)))
        {
            playerBlock.ChangeShield(LevelGrid.GridType.Y);
        }
        if ((Input.GetButtonDown(megaShield) || (useKeyboard && Input.GetKeyDown(KeyCode.Space))) && playerBlock.numberOfShots > 0)
        {
            playerBlock.ChangeShield(LevelGrid.GridType.Mega);
        }
    }

    void Shoot()
    {
        Vector2 move = new Vector2(Input.GetAxis(rHorizontal), Input.GetAxis(rVertical));
        if (useKeyboard && Input.GetKey(KeyCode.LeftShift))
        {
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        if (Input.GetButtonDown(shoot) || (useKeyboard && Input.GetButtonDown("Fire1")))
        {
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y) && Mathf.Abs(move.x) > deadZone)
            {
                playerBlock.Shoot(new Vector2Int((int)Mathf.Sign(move.x), 0));
            }
            else if (Mathf.Abs(move.y) > deadZone)
            {
                playerBlock.Shoot(new Vector2Int(0, (int)Mathf.Sign(move.y)));
            }
        }       
    }
}
