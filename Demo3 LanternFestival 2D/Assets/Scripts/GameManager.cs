using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public bool CanMove { get; private set; }
    public int CurID { get; private set; }

    void Awake(){
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else if (GameManager.instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

	void Start () {
        CanMove = true;
        CurID = 0;
	}
	
	void Update () {
		
	}

    public void SetCanMove(bool _bool)
    {
        CanMove = _bool;
    }

    public void SetCurID(int id)
    {
        CurID = id;
    }
}
